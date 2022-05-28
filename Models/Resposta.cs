namespace NfeToPdf.Models
{
    public class Resposta
    {
        public bool Sucesso { get; set; }
        public byte[] PdfArrayBytes { get; set; }
        public string PdfBase64 { get; set; }
    }
}