﻿using P02_FootballBetting.Data.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P02_FootballBetting.Data.Models
{
    public class Town
    {
        public Town() 
        {
            this.Teams = new HashSet<Team>();
            this.Players = new HashSet<Player>();
        }

        [Key]
        public int TownId { get; set; }

        [Required]
        [MaxLength(ValidationConstants.TownNameMaxLegth)]
        public string Name { get; set; } = null!;

        [ForeignKey(nameof(Country))]
        public int CountryId { get; set; }

        public virtual Country Country { get; set; } = null!;

        public virtual ICollection<Team> Teams { get; set; }

        public virtual ICollection<Player> Players { get; set; }
    }
}
