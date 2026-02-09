namespace Application.Abstractions
{
	public static class ApplicationGuard
	{
		public static ApplicationResponse ValidationError(string message)
			=> new()
			{
				IsSuccess = false,
				Status = ApplicationResultStatus.ValidationError,
				Message = message
			};
	}
}
