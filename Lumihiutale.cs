using Jypeli;

public class Lumihiutale : PhysicsObject
{

    private int Osumat;
    private Color[] Varit;
   
    public Lumihiutale(double leveys, double korkeus, Color[] varit)
        : base(leveys, korkeus) 
    {
        Osumat = 0;
        Color = varit[0];
        Varit = varit;
    }

    public void OtaVastaanOsuma()
    {
        Osumat++;   
        if (Osumat >= Varit.Length)
                {
            this.Destroy();
            
            return;
          
        }
        Color = Varit[Osumat];

    }

}


