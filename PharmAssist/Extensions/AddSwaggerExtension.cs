namespace PharmAssist.Extensions
{
	public static class AddSwaggerExtension
	{
		public static WebApplication UseSwaggerMiddleWares(this WebApplication app)
		{
			app.UseSwagger();
			app.UseSwaggerUI();
			return app;
		}
	}
}
