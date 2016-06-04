﻿using System.IO;

namespace Tangzx.ABSystem
{
    class AssetBundleDataBinaryReader : AssetBundleDataReader
    {
        public override void Read(Stream fs)
        {
            if (fs.Length < 4) return;

            BinaryReader sr = new BinaryReader(fs);
            char[] fileHeadChars = sr.ReadChars(4);
            //读取文件头判断文件类型，ABDB 意思即 Asset-Bundle-Data-Binary
            if (fileHeadChars[0] != 'A' || fileHeadChars[1] != 'B' || fileHeadChars[2] != 'D' || fileHeadChars[3] != 'B')
                return;

            int namesCount = sr.ReadInt32();
            string[] names = new string[namesCount];
            for (int i = 0; i < namesCount; i++)
            {
                names[i] = sr.ReadString();
            }

            while (true)
            {
                if (fs.Position == fs.Length)
                    break;

                string name = names[sr.ReadInt32()];
                string shortFileName = sr.ReadString();
                string hash = sr.ReadString();
                int typeData = sr.ReadInt32();
                int depsCount = sr.ReadInt32();
                string[] deps = new string[depsCount];

                if (!shortName2FullName.ContainsKey(shortFileName))
                    shortName2FullName.Add(shortFileName, name);
                for (int i = 0; i < depsCount; i++)
                {
                    deps[i] = names[sr.ReadInt32()];
                }

                AssetBundleData info = new AssetBundleData();
                info.hash = hash;
                info.fullName = name;
                info.shortName = shortFileName;
                info.dependencies = deps;
                info.compositeType = (AssetBundleExportType)typeData;
                infoMap[name] = info;
            }
            sr.Close();
        }
    }
}