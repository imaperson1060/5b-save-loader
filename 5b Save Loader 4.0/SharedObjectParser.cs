using System;
using System.IO;
using System.Collections.Generic;

public class SharedObject
{
    public List<SOValue> values;

    public SharedObject()
    {
        values = new List<SOValue>();
    }

    public SOValue Get(string keyword)
    {
        for (int i = 0; i < values.Count; i++)
        {
            if (values[i].key == keyword)
            {
                return values[i];
            }
        }
        return new SOValue();   //Return UNDEFINED
    }
}

public struct SOValue
{
    public string key;
    public byte type;
    public string string_val;
    public bool bool_val;
    public int int_val;
    public double double_val;
    public bool[] array_val;
}

class SOReader
{
    public byte[] file_data;
    public int file_size;
    public int pos;

    public SOReader(string filename)
    {
        file_data = File.ReadAllBytes(filename);
        file_size = 0;
        pos = 0;
    }

    public byte Read8()
    {
        return file_data[pos++];
    }

    public UInt16 Read16()
    {
        UInt16 val = file_data[pos++];
        val = (UInt16)((val << 8) | file_data[pos++]);
        return val;
    }

    public UInt32 Read32()
    {
        UInt32 val = 0;
        for (int i = 0; i < 4; i++)
        {
            val = (UInt32)((val << 8) | file_data[pos++]);
        }
        return val;
    }

    public double ReadDouble()
    {
        byte[] double_raw = new byte[8];
        for (int i = 0; i < 8; i++)
        {
            double_raw[i] = file_data[pos + 7 - i];
        }
        pos += 8;
        double val = BitConverter.ToDouble(double_raw, 0);
        return val;
    }

    public string ReadString(int length)
    {
        string val = System.Text.Encoding.UTF8.GetString(file_data, pos, length);
        pos += length;
        return val;
    }
}

struct SOHeader
{
    public UInt16 padding1;
    public UInt32 file_size;
    public string so_type;
    public UInt16 padding2;
    public UInt32 padding3;
}

struct SOTypes
{
    public const byte TYPE_NUMBER = 0x00;
    public const byte TYPE_BOOL = 0x01;
    public const byte TYPE_ARRAY = 0x08;
}

public class SharedObjectParser
{
    public static SharedObject Parse(string filename, SharedObject so = null)
    {
        if (so == null)
        {
            so = new SharedObject();
        }
        if (!File.Exists(filename))
        {
            Console.WriteLine("SharedObject " + filename + " doesn't exist.");
            return so;
        }
        SOReader file = new SOReader(filename);
        List<string> string_table = new List<string>();

        SOHeader header = new SOHeader();
        header.padding1 = file.Read16();
        header.file_size = file.Read32();
        file.file_size = (int)header.file_size + 6;
        header.so_type = file.ReadString(4);
        header.padding2 = file.Read16();
        header.padding3 = file.Read32();

        UInt16 so_name_length = file.Read16();
        string so_name = file.ReadString(so_name_length);
        UInt32 padding4 = file.Read32();

        while (file.pos < file.file_size)
        {
            SOValue so_value = new SOValue();

            UInt16 length_int = file.Read16();
            so_value.key = file.ReadString((int)length_int);

            so_value.type = file.Read8();
            if (so_value.type == SOTypes.TYPE_NUMBER)
            {
                so_value.int_val = (int)file.ReadDouble();
            }
            else if (so_value.type == SOTypes.TYPE_BOOL)
            {
                if (file.Read8() == 1)
                {
                    so_value.bool_val = true;
                }
                else
                {
                    so_value.bool_val = false;
                }
            }
            else if (so_value.type == SOTypes.TYPE_ARRAY)
            {
                UInt32 arr_length = file.Read32();
                bool[] arr = new bool[arr_length];

                for (var i = 0; i < arr_length; i++)
                {
                    UInt16 name_length = file.Read16();
                    string name = file.ReadString(name_length);

                    so_value.type = file.Read8();
                    if (so_value.type == SOTypes.TYPE_BOOL)
                    {
                        if (file.Read8() == 1)
                        {
                            so_value.bool_val = true;
                        }
                        else
                        {
                            so_value.bool_val = false;
                        }
                    }

                    arr[i] = so_value.bool_val;
                    so_value.array_val = arr;
                    so.values.Add(so_value);
                }

                file.Read16();
                file.Read8();
            }
            else
            {
                while (file.pos < file.file_size)
                {
                    byte next_byte = file.Read8();
                    if (next_byte == 0)
                    {
                        --file.pos;
                        break;
                    }
                }
            }
            so.values.Add(so_value);
            if (file.pos < file.file_size)
            {
                file.Read8();
            }
        }
        return so;
    }
}