namespace Domain.Banking.Account.ValueObjects
{
	public class PaymentSettings
	{
		public BatchSettings? Batch { get; private set; }
		public SingleSettings? Single { get; private set; }


		internal PaymentSettings()
		{
			
		}

		public void SetBatchSettings(BatchSettings batchSettings) => Batch = batchSettings;
		public void SetSingleSettings(SingleSettings? singleSettings) => Single = singleSettings;	

	}
}
