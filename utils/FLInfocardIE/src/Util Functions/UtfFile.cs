using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DAM
{

    class UtfFile
    {
        string filePath = "";
        public List<string> names = new List<string>();
        public List<string> hardpoints = new List<string>();

        public UtfFile(string filePath)
        {
            this.filePath = filePath;
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            byte[] buf = new byte[fs.Length];
            fs.Read(buf, 0, (int)fs.Length);
            fs.Close();

            int pos = 0;
            int sig = BitConverter.ToInt32(buf, pos); pos+=4;
            int ver = BitConverter.ToInt32(buf, pos); pos+=4;
	        if (sig != 0x20465455 || ver != 0x101)
                throw new Exception("Unsupported");

            // get node chunk info
            int nodeBlockOffset = BitConverter.ToInt32(buf, pos); pos+=4;
            int nodeSize = BitConverter.ToInt32(buf, pos); pos+=12;

            // get string chunk info
            int stringBlockOffset = BitConverter.ToInt32(buf, pos); pos+=4;
            int stringBlockSize = BitConverter.ToInt32(buf, pos); pos+=8;

            // get data chunk info
            int dataBlockOffset = BitConverter.ToInt32(buf, pos); pos+=4;
            int dataBlockSize = buf.Length - dataBlockOffset;

            // Start at the root node.
            string depth = "+";
            parseNode(buf, nodeBlockOffset, 0, stringBlockOffset, dataBlockOffset, depth);
        }

        void parseNode(byte[] buf, int nodeBlockStart, int nodeStart, int stringBlockOffset, int dataBlockOffset, string depth)
        {
            int offset = nodeBlockStart + nodeStart;

            while (true)
            {
                int nodeOffset = offset;

                int peerOffset = BitConverter.ToInt32(buf, offset); offset += 4;
                int nameOffset = BitConverter.ToInt32(buf, offset); offset += 4;
                int flags = BitConverter.ToInt32(buf, offset); offset += 4;
                int zero = BitConverter.ToInt32(buf, offset); offset += 4;
                int childOffset = BitConverter.ToInt32(buf, offset); offset += 4;
                int allocated_size = BitConverter.ToInt32(buf, offset); offset += 4;
                int size = BitConverter.ToInt32(buf, offset); offset += 4;
                int size2 = BitConverter.ToInt32(buf, offset); offset += 4;
                int u1 = BitConverter.ToInt32(buf, offset); offset += 4;
                int u2 = BitConverter.ToInt32(buf, offset); offset += 4;
                int u3 = BitConverter.ToInt32(buf, offset); offset += 4;

                // Extract the node name
                int len = 0;
                for (int i = stringBlockOffset + nameOffset; i < buf.Length && buf[i] != 0; i++, len++) ;
                string name = System.Text.Encoding.ASCII.GetString(buf, stringBlockOffset + nameOffset, len);

                if (depth.EndsWith("Hardpoints+Fixed+") || depth.EndsWith("Hardpoints+Revolute+"))
                {
                    hardpoints.Add(name);
                }

                if (childOffset > 0 && flags==0x10)
                    parseNode(buf, nodeBlockStart, childOffset, stringBlockOffset, dataBlockOffset, depth + name + "+");

                if (peerOffset > 0)
                    parseNode(buf, nodeBlockStart, peerOffset, stringBlockOffset, dataBlockOffset, depth);

                if ((flags & 0x80) == 0x80)
                    break;

                break;
            }
        }

    }

}
