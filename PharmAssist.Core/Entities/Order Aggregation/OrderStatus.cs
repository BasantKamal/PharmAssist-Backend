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
		[EnumMember(Value ="Pending")]
		Pending,
		[EnumMember(Value = "Out for delivery")]
		OutForDelivery,
		[EnumMember(Value = "Delivered")]
		Delivered,
		[EnumMember(Value = "Cancelled")]
		Cancelled
	}
}
