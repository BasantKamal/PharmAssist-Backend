using PharmAssist.Core.Entities;
using System.Linq.Expressions;


namespace PharmAssist.Core.Specifications
{
	public class BaseSpecifications<T> : ISpecifications<T> where T : BaseEntity
	{
		public Expression<Func<T, bool>> Criteria { get ; set ; }
		public List<Expression<Func<T, object>>> Includes { get ; set; } = new List<Expression<Func<T, object>>>();
		public Expression<Func<T, object>> OrderBy { get; set; }
		public Expression<Func<T, object>> OrderByDesc { get; set; }
		public int Take { get ; set ; }
		public int Skip { get; set ; }
		public bool IsPaginationEnabled { get ; set; }


		//GET ALL
		public BaseSpecifications()
        {
        }

        public BaseSpecifications(Expression<Func<T, bool>>criteriaExpression)
        {
            Criteria = criteriaExpression;
		}

		public void AddOrderBy(Expression<Func<T, object>> OrderByExpression)
		{
			OrderBy= OrderByExpression;
		}
		public void AddOrderByDesc(Expression<Func<T, object>> OrderByDescExpression)
		{
			OrderByDesc = OrderByDescExpression;
		}

		public void ApplyPagination(int skip,int take)
		{
			IsPaginationEnabled = true;
			Skip = skip;
			Take = take;
		}
	}
}
