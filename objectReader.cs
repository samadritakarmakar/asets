using System.Collections.Generic;

public class Var
{
  //0 is double, 1 is string, 2 is array, 3 is object
  public int varType;
  public string data;
  public string varName = "root";
}

public class Object
{
  public int MaxNoOfObjects= 0;
  public Var RootObject= new Var();
  public List<Var> RootObjectList = new List<Var>();
  public Object(ref System.IO.StreamReader jsonFile)
  {
    _jsonFile=jsonFile;
    if(checkObjectStart())
    {
      //RootObject = new Var();
      RootObject.data = ReadOpenToClose('{','}', true);
      RootObject.varType = 3;
      RootObjectList.Add(RootObject);

    }
    else
    {
      GoTillEOF();
    }
  }

  private void GoTillEOF()
  {
    /*do
    {
      RootObject.varName=ReadVarName();
      //System.Console.WriteLine("\nvarName = {0}", RootObject.varName);
      RootObject.data= varReadData();
      RootObjectList.Add(RootObject);
      //System.Console.WriteLine("\n_varData = {0}", RootObject.data);
      RootObject =new Var();
      System.Console.Write("Peek = {0} ", _jsonFile.Peek());
      if(_jsonFile.Peek()==-1)
      {
        stop = true;
      }
    } while (_jsonFile.Peek()!=-1);*/
    string str_tmp = ReadVarName();
    while (str_tmp != null && _jsonFile.Peek()!=-1)
    {
      RootObject =new Var();
      RootObject.varName= str_tmp;
      RootObject.data= varReadData();
      RootObjectList.Add(RootObject);
      str_tmp = ReadVarName();
    }
  }

  public string varReadData()
  {
    string _varData="";
    if(checkVarStart())
    {
      _jsonFile.Read();
    }
    if (checkObjectStart())
    {
      _varData = ReadOpenToClose('{','}', true);
    }
    else if (checkArrayStart())
    {
      _varData = ReadOpenToClose('[',']', false);
    }
    else if (checkStringStartEnd())
    {
      _varData = ReadWithinQuotes(true);
    }
    else if(checkNumber())
    {
      RootObject.varType = 0;
      _varData = ReadTillComma();
    }
    if (checkVarEnd())
    {
      _jsonFile.Read();
    }
    return _varData;
  }

  private string ReadVarName()
  {
    string _varName ="";
    _varName= ReadWithinQuotes(true);
    /*if(_varName!=null)
    {
      RootObject =new Var();
    }*/
    return _varName;
  }

  private bool checkObjectStart()
  {

    char[] Marker ={'{'};
    bool check = checkChar(Marker);
    if (check)
    {
      MaxNoOfObjects++;
      //RootObject =new Var();
      RootObject.varType = 3;
    }
    return check;
  }

  private bool checkObjectEnd()
  {
    char[] Marker ={'}'};
    return checkChar(Marker);
  }

  private bool checkArrayStart()
  {
    char[] Marker ={'['};
    bool check = checkChar(Marker);
    if (check)
    {
      RootObject.varType = 2;
    }
    return check;
  }

  private bool checkArrayEnd()
  {
    char[] Marker ={']'};
    return checkChar(Marker);
  }

  private bool checkStringStartEnd()
  {
    char[] Marker ={'"'};
    bool check = checkChar(Marker);
    if (check)
    {

      RootObject.varType = 1;
    }
    return check;
  }

  private bool checkVarStart()
  {
    char[] Marker ={':'};
    return checkChar(Marker);
  }

  private bool checkVarEnd()
  {
    char[] Marker ={','};
    return checkChar(Marker);
  }

  private bool checkChar(char[] Marker)
  {
    for (int i=0; i< Marker.Length; i++)
    {
      if (_jsonFile.Peek()==(int)Marker[i])
      {
        //System.Console.Write((char)_jsonFile.Peek());
        return true;
      }
    }
    return false;
  }

  private bool checkNumber()
  {
    //System.Console.Write(_jsonFile.Peek());
    if((_jsonFile.Peek()>=48 && _jsonFile.Peek()<=122) || ((char)_jsonFile.Peek()=='-'))
    {
      //System.Console.Write((char)_jsonFile.Peek());
      return true;
    }
    return false;
  }

  private string ReadOpenToClose(char openChar, char closeChar, bool skipOpenCloseChar = false)
  {
    int blockCounter =0;
    char[] ch={'0'};
    string data="";
    do
    {
      Skip_Space_NewLine();
      ch[0]=(char)_jsonFile.Read();
      Skip_Space_NewLine();
      if(openChar == ch[0])
      {
        //System.Console.Write("GetsIn");
        blockCounter++;
        if(skipOpenCloseChar && blockCounter == 1)
        {
          ch[0]='\0';
        }
      }
      else if (closeChar == ch[0])
      {
        blockCounter--;
        if(skipOpenCloseChar && blockCounter == 0)
        {
          ch[0]='\0';
        }
      }
      data=data+new string(ch);
      //System.Console.Write("bC=  {0}",blockCounter);
      //System.Console.Write(" dataTemp{0}", dataTemp);
    } while (blockCounter>0);
    return data;
  }

  private string ReadWithinQuotes(bool skipOpenCloseChar = false)
  {
    int blockCounter=0;
    string data = "";
    char [] ch ={'0'};
    int c;
    do
    {
      ch[0] = (char)_jsonFile.Read();
      c = (int)ch[0];
      //System.Console.Write(" c = {0}", c);
      if(ch[0] == '"')
      {
        blockCounter++;
        if(skipOpenCloseChar)
        {
          ch[0]='\0';
        }
      }
      data = data+new string(ch);
      //System.Console.WriteLine("bC = {0}", blockCounter);
      //System.Console.WriteLine("condition = {0}", (blockCounter%2!=0 && c==0));
    } while (blockCounter%2!=0 || c==0);
    //System.Console.WriteLine("data within quotes = {0}",data);
    return data;
  }

  private string ReadTillComma()
  {
    //System.Console.WriteLine("ReadTillComma is used");
    string data="";
    char[] ch = {(char)_jsonFile.Read()};
    while (ch[0]!=',')
    {
      //System.Console.Write("ch[0] = {0} ", ch[0]);
      data = data+new string(ch);
      ch[0] = (char)_jsonFile.Read();
    }
    //System.Console.WriteLine("ReadTillComma = {0}", data);
    return data;
  }

  private void Skip_Space_NewLine()
  {
    char ch = (char)_jsonFile.Peek();
    //System.Console.Write(ch);
    while (ch==' ' || ch.Equals('\n') || ch.Equals('\t'))
    {
      _jsonFile.Read();
      ch = (char)_jsonFile.Peek();
    }
  }
  private void SkipTillComma()
  {
    //Skip_Space_NewLine();
    char ch = (char)_jsonFile.Peek();
    while (ch==',')
    {
      _jsonFile.Read();
      ch = (char)_jsonFile.Peek();
    }
  }

  private System.IO.StreamReader _jsonFile;
}



public class ObjectReader
{
  public static void Main(string[] args)
    {
      System.IO.StreamReader jsonFileRoot = new System.IO.StreamReader("data.json");
      Object MeshData= new Object(ref jsonFileRoot);
      List <Object> MeshDataList = new List <Object> ();
      MeshDataList.Add(MeshData);
      int TotalObjects =1;
      for (int i=0; i<=TotalObjects; i++)
      {
        TotalObjects =MeshDataList[i].MaxNoOfObjects;
        //string data = MeshDataList[i].RootObjectList[i].data;
        for (int j=0; j<MeshDataList[i].RootObjectList.Count; j++)
        {
          string data = MeshDataList[i].RootObjectList[j].data;
          jsonFileRoot = StringToStreamReader(data);
          MeshData = new Object(ref jsonFileRoot);
          MeshDataList.Add(MeshData);

        }
        //printData(MeshDataList, i);
        /*System.Console.WriteLine("Continue?(y/n)");
        string option = System.Console.ReadLine();
        if ("n"== option)
        {
          break;
        }*/
      }
      printData(MeshDataList, 0);
    }

    public static System.IO.StreamReader StringToStreamReader(string data)
    {
      byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(data);
      System.IO.MemoryStream stream = new System.IO.MemoryStream( byteArray );
      System.IO.StreamReader jsonFileRoot = new System.IO.StreamReader(stream);
      return jsonFileRoot;
    }

    public static void printData(List <Object> MeshDataList, int startMeshPoint)
    {
      int [] Traversal = {0, 1, 2, 4, 2, 5, 2, 6, 1, 3, 7, 3, 8};
      int [] start =     {0, 0, 0, 0, 1, 0, 2, 0, 1, 0, 0, 1, 0};
      int [] end =       {0, 0, 0, 4, 1, 4, 2, 4, 1, 0, 6, 1, 6};
      string filename = "geometry.txt";
      string text = "";
      //System.IO.StreamWriter sw = new System.IO.StreamWriter("geometry.txt", true);
      using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filename, false))
      {
        sw.Write("");
      }


      using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filename, true))
      {
        for (int traverse =0; traverse<Traversal.Length; traverse++)
        {
          int i = Traversal[traverse];

          //System.Console.WriteLine("\nMeshDataList {0}", i);
          //System.Console.WriteLine("start = {0}, end ={1}", start[traverse], end[traverse]);
          for (int j=start[traverse]; j<=end[traverse]; j++)
          {
            //System.Console.WriteLine("Root Level ={0}", j);
            System.Console.Write("{0}: ", MeshDataList[i].RootObjectList[j].varName);
            text= MeshDataList[i].RootObjectList[j].varName + ": ";
            sw.Write(text);
            //System.Console.WriteLine("{0}varType ={1}", tab, MeshDataList[i].RootObjectList[j].varType);
            if (MeshDataList[i].RootObjectList[j].varType !=3)
            {
              System.Console.WriteLine("{0}", MeshDataList[i].RootObjectList[j].data);
              text= MeshDataList[i].RootObjectList[j].data;
              sw.WriteLine(text);
            }
            else
            {
              System.Console.Write("\n");
              sw.Write("\n");
            }
          }
        }
      }

    }
}
