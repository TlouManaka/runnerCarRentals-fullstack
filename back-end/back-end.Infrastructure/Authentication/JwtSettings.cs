namespace backend.Infrastructure.Authentication;




public class JwtSettings
{
    
    public const  string SectionName = "JwtSettings";

    public string Secret{ get; init; } = null!;

     public string issuer{ get; init; } = null!;

    public string Audience { get; init; }  = null!;  

    public int ExpryMinutes { get; init;   }

}