using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;

namespace GalaxyBlox.Utils
{
    /// <summary>
    /// Class for serializing and deserializing xml files to/from isolated storage file.
    /// </summary>
    public static class XmlIsoStore
    {
        public static void Serialize<T>(T objectToSerialize, string path)
        {
            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(path, FileMode.Create, iso))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(stream, objectToSerialize);
                }
            }
        }

        public static bool TryDeserialize<T>(out T result, string filePath) where T : new()
        {
            try
            {
                using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(filePath, FileMode.Open, iso))
                    {
                        // Read the data from the file
                        XmlSerializer serializer = new XmlSerializer(typeof(T));
                        result = (T)serializer.Deserialize(stream);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                result = new T();
                return false;
            }
        }
    }
}