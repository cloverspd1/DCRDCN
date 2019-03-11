namespace BEL.DataAccessLayer
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// JSON Helper
    /// </summary>
    public static class JSONHelper
    {
        /// <summary>
        /// To the json.
        /// </summary>
        /// <typeparam name="T">Type of Object</typeparam>
        /// <param name="obj">The object.</param>
        /// <returns>json string</returns>
        public static string ToJSON<T>(T obj)
        {
            //BinaryFormatter bf = new BinaryFormatter();
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    bf.Serialize(ms, obj);
            //    return ms.ToArray();
            //}
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            return JsonConvert.SerializeObject(obj, settings);
        }

        /// <summary>
        /// To the object.
        /// </summary>
        /// <typeparam name="T">Type of Object</typeparam>
        /// <param name="jsonString">The json string.</param>
        /// <returns>Returns Type of Object</returns>
        public static T ToObject<T>(string jsonString)
        {
            //BinaryFormatter bf = new BinaryFormatter();
            //using (var ms = new MemoryStream(byteArray))
            //{
            //    return (T)bf.Deserialize(ms);
            //}
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            };
            if (string.IsNullOrEmpty(jsonString))
            {
                return default(T);
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(jsonString, settings);
            }
        }

        /// <summary>
        /// Serializes the specified object.
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="obj">The object.</param>
        /// <returns>return string</returns>
        public static string Serialize<T>(T obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(stream, obj);
                stream.Position = 0;
                StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Deserializes the specified data.
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="data">The data.</param>
        /// <returns>string return</returns>
        public static T Deserialize<T>(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                return default(T);
            }
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                return (T)serializer.ReadObject(stream);
            }
        }
    }
}
