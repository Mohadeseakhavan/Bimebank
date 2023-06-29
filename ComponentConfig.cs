namespace bimeh_back.Components
{
    public class ComponentConfig
    {
        public Configs.SmsConfig Sms { get; set; }
        public Configs.PaymentConfig Payment { get; set; }
        public Configs.JwtConfig Jwt { get; set; }
        public Configs.FcmConfig Fcm { get; set; }
        public string Environment { get; set; }
        public string Url { get; set; }
        public string AppUrl { get; set; }
    }
}