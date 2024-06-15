using System.ComponentModel.DataAnnotations;

namespace LostFindingApi.Models.Data
{
    public class Liscence
    {
        [Key,MaxLength(20)]
        public string id { get; set; }
        [MaxLength(60)]
        public string name { get; set; }
        [MaxLength(50)]
        public string address {  get; set; }
        [MaxLength(60)]
        public string traffic_unit {  get; set; }
        [MaxLength(20)]
        public string nationality {  get; set; }
        [MaxLength(20)]
        public string job {  get; set; }
        [MaxLength(60)]
        public string name_English {  get; set; }
        [MaxLength(30)]
        public string birth_date {  get; set; }
        [MaxLength(30)]
        public string birth_place {  get; set; }
        [MaxLength(5)]
        public string gender { get; set; }
        [MaxLength(30)]
        public string age {  get; set; }
    }
}
//"id": "28505181801531",
//  "name": "عبدالفتاح عاطف عبداالفتاح رفاعي الجيزاوي",
//  "address": "كومهك حمادة النشتيدي",
//  "traffic_unit": "ادارة مرور البحيرة وحدة مرور كوم حماده",
//  "nationality": "مصرى",
//  "job": "سائلق",
//  "name_in_english": "Abdel Fattah Atef Abd Al -Fattah Rifai Al -Jizawi",
//  "birth_date": "1985-05-18",
//  "birth_place": "البحيرة",
//  "gender": "ذكر",
//  "age": "38 year and 10 month"