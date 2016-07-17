using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace StroopwaffleII_Shared {
    public abstract class Packet {
        public abstract void Pack(NetOutgoingMessage message);
        public abstract void Unpack(NetIncomingMessage message);

        public static T Deserialize<T>(string toDeserialize) {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            StringReader textReader = new StringReader(toDeserialize);
            return (T)xmlSerializer.Deserialize(textReader);
        }

        public string Serialize<T>(T toSerialize) {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            StringWriter textWriter = new StringWriter();
            xmlSerializer.Serialize(textWriter, toSerialize);
            return textWriter.ToString();
        }
    }
}
