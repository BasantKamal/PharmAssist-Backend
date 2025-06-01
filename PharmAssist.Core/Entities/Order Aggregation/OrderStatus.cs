using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PharmAssist.Core.Entities.Order_Aggregation
{
	public enum OrderStatus
	{
		[EnumMember(Value ="Pending")] //lma yegy yt3ml fel db yb2a bel value de
		Pending,
		[EnumMember(Value = "Payment Received")]
		PaymentReceived,
		[EnumMember(Value = "Payment Failed")]
		PaymentFailed
	}
}
