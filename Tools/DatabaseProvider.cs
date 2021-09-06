using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace UniversityAssistantBlazorWasm.Tools
{
    public class DatabaseProvider
    {
        private FirebaseClient firebaseClient { get; set; }
        private readonly string querySeparator = @"/\";

        public DatabaseProvider(string url, FirebaseOptions options)
        {
            firebaseClient = new FirebaseClient(url, options);
        }

        private ChildQuery GetChildQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (firebaseClient is null)
            {
                throw new NullReferenceException(nameof(firebaseClient));
            }

            var splitedQuery = query.Split(querySeparator.ToCharArray());
            var child = firebaseClient.Child(splitedQuery[0]);
            for (var i = 1; i < splitedQuery.Length; i++)
            {
                if (splitedQuery[i].Length <= 0)
                {
                    continue;
                }

                child = child.Child(splitedQuery[i]);
            }

            return child;
        }

        /// <summary>
        /// データベースからデータを取得
        /// </summary>
        /// <typeparam name="T">保存したデータの型</typeparam>
        /// <param name="query">クエリ</param>
        /// <returns>取得したデータ</returns>
        public async Task<T[]> GetAsync<T>(string query)
        {
            var child = GetChildQuery(query);
            var result = await child.OnceAsync<T>();
            return result.Select(o => o.Object).ToArray();
        }

        /// <summary>
        /// データをデータベースに保存<br/>
        /// 保存先: クエリ/ランダムな文字列/データ
        /// </summary>
        /// <typeparam name="T">保存するデータの型</typeparam>
        /// <param name="query">クエリ</param>
        /// <param name="data">保存するデータ</param>
        public async Task PostAsync<T>(string query, T data)
        {
            var child = GetChildQuery(query);
            await child.PostAsync(data);
        }

        /// <summary>
        /// データをデータベースに保存<br/>
        /// 保存先: クエリ/データ
        /// </summary>
        /// <typeparam name="T">保存するデータの型</typeparam>
        /// <param name="query">クエリ</param>
        /// <param name="data">保存するデータ</param>
        public async Task PutAsync<T>(string query, T data)
        {
            var child = GetChildQuery(query);
            await child.PutAsync(data);
        }

        /// <summary>
        /// データベースからデータを削除
        /// </summary>
        /// <param name="query">クエリ</param>
        public async Task DeleteAsync(string query)
        {
            var child = GetChildQuery(query);
            await child.DeleteAsync();
        }
    }
}
