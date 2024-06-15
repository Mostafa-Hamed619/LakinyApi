using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace LostFindingApi.Models.Data
{
    public class Card
    {
        [MaxLength(20)]
        public string id { get; set; }
        [MaxLength(15)]
        public string frist_name { get; set; }
        [MaxLength(50)]
        public string rest_of_name { get; set; }
        [MaxLength(50)]
        public string address { get; set; }
        [MaxLength(50)]
        public string regin { get; set; }
        [MaxLength(30)]
        public string birth_date {  get; set; }
        [MaxLength(50)]
        public string birth_place { get; set; }
        [MaxLength(6)]
        public string gender { get; set; }
        [MaxLength(20)]
        public string age { get; set; }
        [MaxLength(50)]
        public string face { get; set; }
    }
}
