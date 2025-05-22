using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmAssist.Core.Entities.Identity
{
	public class AppUser:IdentityUser
	{
        public string? DisplayName  { get; set; }
		public Address Address { get; set; }
		public string? PromptReason { get; set; }
		public string? HasChronicConditions { get; set; }
		public string? TakesMedicationsOrTreatments { get; set; }
		public string? CurrentSymptoms { get; set; }
	}
}
