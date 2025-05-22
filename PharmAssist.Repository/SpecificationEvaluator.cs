using Microsoft.EntityFrameworkCore;
using PharmAssist.Core.Entities;
using PharmAssist.Core.Specifications;


namespace PharmAssist.Repository
{
	public static class SpecificationEvaluator<T> where T : BaseEntity
	{
		public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecifications<T> Spec)
		{
			var Query = inputQuery;  

			if (Spec.Criteria is not null)   
			{
				Query = Query.Where(Spec.Criteria);
			}
			if(Spec.OrderBy is not null)
			{
				Query = Query.OrderBy(Spec.OrderBy);	
			}
			if (Spec.OrderByDesc is not null)
			{
				Query = Query.OrderByDescending(Spec.OrderByDesc);
			}
			if (Spec.IsPaginationEnabled)
			{
				Query=Query.Skip(Spec.Skip).Take(Spec.Take);
			}

			Query = Spec.Includes.Aggregate(Query, (CurrentQuery, IncludeExpression) => CurrentQuery.Include(IncludeExpression));
 
			return Query;
		}
	}
}
