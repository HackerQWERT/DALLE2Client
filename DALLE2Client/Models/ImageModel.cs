
namespace DALLE2Client.Models;

public  class ImageModel
{

    [PrimaryKey,AutoIncrement]public int Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set;  }
    [Ignore]public ImageSource ImageSource{ get; set; }
    public byte[] ImageBytes{ get; set; }
    public DateTime DateTime { get; set; }    
}


