using RMUD;

namespace Akkoteaque
{

    public class Start : RMUD.Room
    {
        public override void Initialize()
        {
            Short = "Start Room";

            Move(new MudObject("cipher", Cipher.EncodeParagraph("The quick", "brown fox", "jumped over", "the lazy dog.")), this);
        }
    }   
}