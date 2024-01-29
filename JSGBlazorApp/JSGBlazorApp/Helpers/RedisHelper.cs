using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JSGBlazorApp.Helpers
{
    public class RedisHelper
    {
        // Lazy<T>를 사용하여 스레드 안전성을 보장하는 싱글턴 인스턴스 생성
        private static readonly Lazy<RedisHelper> lazy = new Lazy<RedisHelper>(() => new RedisHelper());

        // 싱글턴 인스턴스에 접근할 수 있는 public 속성
        public static RedisHelper Instance => lazy.Value;

        private ConnectionMultiplexer redisConn;

        // 현재 연결되어 있는 EndPoint 정보를 저장하는 속성
        public string ConnectionEndPoint { get; private set; }

        // 현재 Redis 서버와 연결 상태인지 확인하는 속성
        private bool IsConnected => redisConn != null && redisConn.IsConnected;

        const int MAX_RETRY_COUNT = 2;

        /// <summary>
        /// 주어진 연결 정보로 Redis 서버와의 연결 시도. 성공시 true, 실패시 false 반환.
        /// </summary>
        /// <param name="connectionString">URL</param>
        /// <returns></returns>
        public bool Connect(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return false;
            }

            int retryCount = 0;
            while (retryCount < MAX_RETRY_COUNT) // 최대 3회 재시도
            {
                try
                {
                    var configOptions = new ConfigurationOptions
                    {
                        EndPoints = { connectionString },
                        AbortOnConnectFail = false
                    };

                    redisConn?.Dispose();

                    redisConn = ConnectionMultiplexer.Connect(configOptions);

                    // 연결 실패 이벤트 핸들러 등록. 연결이 끊어졌을 때 Reconnect 메소드 호출.

                    if (IsConnected)
                    {
                        redisConn.ConnectionFailed += delegate
                        {
                            Reconnect();
                        };

                        return true; // 연결에 성공했으므로 true 반환.
                    }
                    else
                    {
                        retryCount++;
                    }
                }
                catch
                {
                    retryCount++; // 재시도 횟수 증가.
                }
            }

            return false; // 최대 재시도 횟수를 초과하여 연결에 실패했으므로 false 반환.
        }

        /// <summary>
        /// 현재 연결상태 값 반환
        /// </summary>
        /// <returns></returns>
        public bool CheckConnectionStatus()
        {
            return IsConnected;
        }

        /// <summary>
        /// 주어진 키로 N번째 데이터베이스에서 값을 가져오는 메소드.
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <param name="dbNumber">Database number</param>
        /// <returns></returns>
        public string GetString(string key, int dbNumber = 0)
        {
            if (!IsConnected)
            {
                return "";
            }

            var db = redisConn.GetDatabase(dbNumber);
            return db.StringGet(key);
        }

        /// <summary>
        /// 주어진 해시 키로 N번째 데이터베이스에서 값을 가져오는 메소드.
        /// </summary>
        /// <param name="hashId">Redis Hash ID</param>
        /// <param name="key">Redis Key</param>
        /// <param name="dbNumber">Database number</param>
        public string GetHashString(string hashId, string key, int dbNumber = 0)
        {
            if (!IsConnected)
            {
                return "";
            }

            var db = redisConn.GetDatabase(dbNumber);
            return db.HashGet(hashId, key);
        }

        public void SetHash(string key, string hkey, string hval, int dbNumber = 0)
        {
            try
            {
                var db = redisConn.GetDatabase(dbNumber);
                db.HashSet(key, hkey, hval);
            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// 주어진 해시 키로 N번째 데이터베이스에서 값을 삭제하는 메소드.
        /// </summary>
        /// <param name="hashId">Redis Hash ID</param>
        /// <param name="key">Redis Key</param>
        /// <param name="dbNumber">Database number</param>
        public bool DeleteHashKey(string hashId, string key, int dbNumber = 0)
        {
            if (!IsConnected)
            {
                return false;
            }

            var db = redisConn.GetDatabase(dbNumber);
            return db.HashDelete(hashId, key);
        }

        /// <summary>
        /// 현재 연결이 끊어져 있고 EndPoint 정보가 유효하다면 재연결 시도.
        /// </summary>
        private void Reconnect()
        {
            if (!IsConnected && !string.IsNullOrEmpty(ConnectionEndPoint))
            {
                Connect(ConnectionEndPoint);
            }
        }

        /// <summary>
        /// 새로운 EndPoint로의 변경 및 재연결을 수행하는 메소드.
        /// </summary>
        /// <param name="newConnectionString">URL</param>
        public bool ChangeConnection(string newConnectionString)
        {
            if (!string.IsNullOrEmpty(newConnectionString))
            {
                ConnectionEndPoint = newConnectionString;
                return Connect(ConnectionEndPoint);
            }
            else
            {
                return false;
            }
        }
    }
}
