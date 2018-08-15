using System;
using System.IO;
using System.Text;
using FullSerializer;
using UnityEngine;

namespace BayatGames.SaveGameFree.Serializers
{
	/// <summary>
	///     Save Game Json Serializer.
	/// </summary>
	public class SaveGameJsonSerializer : ISaveGameSerializer
    {
	    /// <summary>
	    ///     Serialize the specified object to stream with encoding.
	    /// </summary>
	    /// <param name="obj">Object.</param>
	    /// <param name="stream">Stream.</param>
	    /// <param name="encoding">Encoding.</param>
	    /// <typeparam name="T">The 1st type parameter.</typeparam>
	    public void Serialize<T>(T obj, Stream stream, Encoding encoding)
        {
#if !UNITY_WSA || !UNITY_WINRT
            try
            {
                var writer = new StreamWriter(stream, encoding);
                var serializer = new fsSerializer();
                var data = new fsData();
                serializer.TrySerialize(obj, out data);
                writer.Write(fsJsonPrinter.CompressedJson(data));
                writer.Dispose();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
#else
			StreamWriter writer = new StreamWriter ( stream, encoding );
			writer.Write ( JsonUtility.ToJson ( obj ) );
			writer.Dispose ();
			#endif
        }

	    /// <summary>
	    ///     Deserialize the specified object from stream using the encoding.
	    /// </summary>
	    /// <param name="stream">Stream.</param>
	    /// <param name="encoding">Encoding.</param>
	    /// <typeparam name="T">The 1st type parameter.</typeparam>
	    public T Deserialize<T>(Stream stream, Encoding encoding)
        {
            var result = default(T);
#if !UNITY_WSA || !UNITY_WINRT
            try
            {
                var reader = new StreamReader(stream, encoding);
                var serializer = new fsSerializer();
                var data = fsJsonParser.Parse(reader.ReadToEnd());
                serializer.TryDeserialize(data, ref result);
                if (result == null) result = default(T);
                reader.Dispose();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
#else
			StreamReader reader = new StreamReader ( stream, encoding );
			result = JsonUtility.FromJson<T> ( reader.ReadToEnd () );
			reader.Dispose ();
			#endif
            return result;
        }
    }
}