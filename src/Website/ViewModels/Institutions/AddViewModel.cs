﻿namespace QOAM.Website.ViewModels.Institutions
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    using QOAM.Core;

    public class AddViewModel
    {
        [Required]
        [StringLength(1000)]
        public string Name { get; set; }

        [Required]
        [StringLength(1000)]
        [DisplayName("Domainname")]
        public string ShortName { get; set; }

        public Institution ToInstitution()
        {
            return new Institution
            {
                Name = this.Name, 
                ShortName = this.ShortName.Replace("http://", string.Empty).Replace("www.", string.Empty),
            };
        }
    }
}