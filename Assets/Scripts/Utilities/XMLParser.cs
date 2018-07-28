using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Security.Cryptography;
using System.Text;

public static class XMLParser
{
    public static void EncryptXMLFile(string srcFile, string dstFile)
    {
        //get fileinfo of source to use file size
        FileInfo info = new FileInfo(srcFile);

        //create a new file stream to open source file and read it
        FileStream input = new FileStream(srcFile, FileMode.Open, FileAccess.Read);

        //create data buffer using file size to read and store entire file
        byte[] dataBuffer = new byte[info.Length];

        //read the file into buffer and then close the stream
        input.Read(dataBuffer, 0, (int)info.Length);
        input.Close();

        //create a new file stream to create the new file and write to it
        FileStream output = new FileStream(dstFile, FileMode.Create, FileAccess.Write);

        //create encryption object with custom key
        DESCryptoServiceProvider encrypt = new DESCryptoServiceProvider();
        encrypt.Key = ASCIIEncoding.ASCII.GetBytes("56@SUD7P");
        encrypt.IV = ASCIIEncoding.ASCII.GetBytes("0Z8#FLP1");

        //create encryption stream using the output stream and write to file
        CryptoStream cStream = new CryptoStream(output, encrypt.CreateEncryptor(), CryptoStreamMode.Write);
        cStream.Write(dataBuffer, 0, dataBuffer.Length);

        //close all streams
        cStream.Close();
        output.Close();
    }

    public static string LoadEncryptedXMLFile(string srcFile)
    {
        StringBuilder file = new StringBuilder();

        //create decryption object with same key
        DESCryptoServiceProvider decrypt = new DESCryptoServiceProvider();
        decrypt.Key = ASCIIEncoding.ASCII.GetBytes("56@SUD7P");
        decrypt.IV = ASCIIEncoding.ASCII.GetBytes("0Z8#FLP1");

        //create input stream and pass it through the decryption stream
        FileStream iStream = new FileStream(srcFile, FileMode.Open, FileAccess.Read);
        CryptoStream cStream = new CryptoStream(iStream, decrypt.CreateDecryptor(), CryptoStreamMode.Read);

        //create stream reader to read input line by line
        StreamReader sReader = new StreamReader(cStream, Encoding.ASCII);

        //store each line of file in list of strings
        while (!sReader.EndOfStream)
        {
            string line = sReader.ReadLine();
            file.Append(line);
        }

        //close all streams
        sReader.Close();
        cStream.Close();
        iStream.Close();

        return file.ToString();
    }

    //Load all unique base monster data from file into memory
    public static Dictionary<int, Monster> LoadMonsterData()
    {
        //encrypt original file and then load the encrypted file
        EncryptXMLFile("Assets/Resources/XMLs/Monsters_Original.xml", "Assets/Resources/XMLs/Monsters_Safe.xml");
        string monsterFile = LoadEncryptedXMLFile("Assets/Resources/XMLs/Monsters_Safe.xml");

        //create temporary dictionary to store and return data
        Dictionary<int, Monster> monsterData = new Dictionary<int, Monster>();

        //loop through file and fill collection accordingly
        using (XmlReader reader = XmlReader.Create(new StringReader(monsterFile)))
        {
            //get amount of monsters to add to collection
            reader.ReadToFollowing("Amount");
            int amount = reader.ReadElementContentAsInt();

            //loop through each monster and extract base values
            //create unique base monster for each input and add to collection
            for (int i = 0; i < amount; ++i)
            {
                //get species number used for dictionary key
                reader.ReadToFollowing("ID");
                int key = reader.ReadElementContentAsInt();

                //get species name
                reader.ReadToFollowing("Name");
                string name = reader.ReadElementContentAsString();

                //get 6 base stats and store in array
                reader.ReadToFollowing("Base");
                string[] baseStats = reader.ReadElementContentAsString().Split(',');
                double[] statsArray = new double[6];

                for (int j = 0; j < baseStats.Length; ++j)
                {
                    statsArray[j] = double.Parse(baseStats[j]);
                }

                //get type(s)
                reader.ReadToFollowing("Type");
                string[] types = reader.ReadElementContentAsString().Split(',');
                MonsterType type1 = (MonsterType)Enum.Parse(typeof(MonsterType), types[0]);
                MonsterType type2 = MonsterType.NONE;

                if (types.Length > 1)
                    type2 = (MonsterType)Enum.Parse(typeof(MonsterType), types[1]);

                //get experience group
                reader.ReadToFollowing("EGroup");
                ExperienceGroup eGroup = (ExperienceGroup)reader.ReadElementContentAsInt();

                //create new monster with data and add to collection
                Monster monster = new Monster();
                monster.SetUpInitialValues(key, name, statsArray, type1, type2, eGroup);

                monsterData.Add(key, monster);
            }
        }
        
        return monsterData;
    }
}
