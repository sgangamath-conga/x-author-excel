/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;

namespace Apttus.XAuthor.Core
{
    public class ApttusXmlSerializerUtil
    {
        public static string TargetNamespace
        {
            get { return "http://www.w3.org/2001/XMLSchema"; }
        }

        /// <summary>
        /// Creates an object from an XML string.
        /// </summary>
        /// <param name="Xml"></param>
        /// <param name="ObjType"></param>
        /// <returns></returns>
        public static object FromXml(string Xml, Type ObjType)
        {
            var extraActionTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(Action).IsAssignableFrom(t)).ToList();            
            XmlSerializer ser;
            ser = new XmlSerializer(ObjType, extraActionTypes.ToArray());
            StringReader stringReader;
            stringReader = new StringReader(Xml);
            XmlTextReader xmlReader;
            xmlReader = new XmlTextReader(stringReader);
            object obj;
            obj = ser.Deserialize(xmlReader);
            xmlReader.Close();
            stringReader.Close();
            return obj;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlOfAnObject"></param>
        /// <returns></returns>
        public static object DeSerializeAnObject(string xmlOfAnObject)
        {
            Object obj = new Object();
            StringReader read = new StringReader(xmlOfAnObject);
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            XmlReader reader = new XmlTextReader(read);
            try
            {
                obj = (object)serializer.Deserialize(reader);
                return obj;
            }
            catch
            {
                throw;
            }
            finally
            {
                reader.Close();
                read.Close();
                read.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objToSerialize"></param>
        /// <returns></returns>
        public static string SerializeObject(Object objToSerialize)
        {
            XmlSerializer ser = new XmlSerializer(objToSerialize.GetType());
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);

            ser.Serialize(writer, objToSerialize);
            return writer.ToString();
        }

        /// <summary>
        /// Serializes the <i>Obj</i> to an XML string.
        /// </summary>
        /// <param name="Obj"></param>
        /// <param name="ObjType"></param>
        /// <returns></returns>
        public static string ToXml(object Obj, Type ObjType)
        {
            if (Obj == null)
                return null;

            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.OmitXmlDeclaration = true;
            writerSettings.ConformanceLevel = ConformanceLevel.Auto;
            writerSettings.CloseOutput = true;

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", ""); // empty namespace will omit the namespace 

            StringBuilder sb = new StringBuilder();
            XmlWriter xmlwriter = XmlWriter.Create(sb, writerSettings);

            Type[] extraActionTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && typeof(Action).IsAssignableFrom(t)).ToArray();            
            XmlSerializer xmlSerializer = new XmlSerializer(ObjType, extraActionTypes);
            xmlSerializer.Serialize(xmlwriter, Obj, ns);
            xmlwriter.Flush();
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Obj"></param>
        /// <param name="ObjType"></param>
        /// <param name="fullPath"></param>
        private static void ToFile(object Obj, Type ObjType, string fullPath)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", ""); // empty namespace will omit the namespace 
            Type[] extraTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(Action).IsAssignableFrom(t)).ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(ObjType, extraTypes);
            TextWriter textWriter = new StreamWriter(fullPath);
            xmlSerializer.Serialize(textWriter, Obj, ns);
            textWriter.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ObjType"></param>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        private static Object FromFile(Type ObjType, string fullPath)
        {
            try
            {
                Type []extraTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(Action).IsAssignableFrom(t)).ToArray();
                XmlSerializer reader = new XmlSerializer(ObjType, extraTypes);
                StreamReader file = new StreamReader(fullPath);
                object obj = reader.Deserialize(file);
                file.Close();
                return obj;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string Serialize<T>(T instance)
        {
            return ToXml(instance, instance.GetType());
        }

        public static T Deserialize<T>(string xml)
        {
            return (T)FromXml(xml, typeof(T));
        }

        public static void SerializeToFile<T>(T instance, string path)
        {
            ToFile(instance, instance.GetType(), path);
        }

        public static T SerializeFromFile<T>(string path)
        {
            return (T)FromFile(typeof(T), path);
        }

    }

    
}


