using Application.OrderManagement;
using Application.OrderManagement.Dtos.GroupedOrder;
using CentralPaymentWebApi.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentralPaymentWebApi.Controllers.Payment
{
	[Authorize(Policy = AuthorizationConsts.UserPolicy)]
	public class GroupedPaymentController : ApiControllerBase
	{
		private readonly IGroupedOrderApplication _groupedOrderApp;

		public GroupedPaymentController(IGroupedOrderApplication groupedOrderApp)
		{
			_groupedOrderApp = groupedOrderApp ?? throw new ArgumentNullException(nameof(groupedOrderApp));
		}

		/// <summary>
		/// ایجاد دستور پرداخت گروهی جدید
		/// </summary>
		/// <param name="createDto"></param>
		/// <returns></returns>
		[HttpPost(RouteTemplates.Create)]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
		public async Task<IActionResult> CreateAsync(CreateGroupedOrderDto createDto)
		{
			try
			{
				var appResponse = await _groupedOrderApp.CreateAsync(createDto);
				return HandleOutput(appResponse);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}


		/// <summary>
		/// افزودن تراکنش های دستور پرداخت
		/// </summary>
		/// <param name="groupedTransactionDto"></param>
		/// <returns></returns>
		[HttpPost("addTransactions")]
		[ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
		public async Task<IActionResult> AddTransactionsAsync(AddGroupedTransactionDto groupedTransactionDto)
		{
			try
			{
				var appResponse = await _groupedOrderApp.AddTransactions(groupedTransactionDto);
				return HandleOutput(appResponse);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// حذف تراکنش از دستور پرداخت بر اساس شناسه
		/// </summary>
		/// <param name="orderId"></param>
		/// <param name="transactionId"></param>
		/// <returns></returns>
		[HttpPost("{orderId}/transaction/{transactionId}/remove")]
		[ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
		public async Task<IActionResult> RemoveTransactionAsync(Guid orderId, Guid transactionId)
		{
			try
			{
				var appResponse = await _groupedOrderApp.RemoveTransaction(orderId, transactionId);
				return HandleOutput(appResponse);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// نهایی کردن دست.ور پرداخت قبل از ارسال به بانک
		/// </summary>
		/// <param name="orderId"></param>
		/// <returns></returns>
		[HttpPost("{orderId}/finalize")]
		[ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
		public async Task<IActionResult> FinalizeOrderAsync(Guid orderId)
		{
			try
			{
				var appResponse = await _groupedOrderApp.FinalizeOrder(orderId);
				return HandleOutput(appResponse);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}


		/// <summary>
		/// ارسال دستور پرداخت به بانک جهت پردازش
		/// </summary>
		/// <param name="orderId"></param>
		/// <returns></returns>
		[HttpPost("{orderId}/send")]
		[ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
		public async Task<IActionResult> SendToBankAsync(Guid orderId)
		{
			try
			{
				var appResponse = await _groupedOrderApp.SendOrderAsync(orderId);
				return HandleOutput(appResponse);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}


		/// <summary>
		/// استعلام از دستور پرداخت
		/// </summary>
		/// <param name="orderId"></param>
		/// <returns></returns>
		[HttpPost("{orderId}/inquiry")]
		[ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
		public async Task<IActionResult> InquiryOrderAsync(Guid orderId)
		{
			try
			{
				var appResponse = await _groupedOrderApp.InquiryPaymentOrder(orderId);
				return HandleOutput(appResponse);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// استعلام از تراکنش دستور پرداخت
		/// </summary>
		/// <param name="orderId"></param>
		/// <returns></returns>
		[HttpPost("{orderId}/transaction/{transactionId}/inquiry")]
		[ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
		public async Task<IActionResult> InquiryTransactionAsync(Guid orderId, Guid transactionId)
		{
			try
			{
				var appResponse = await _groupedOrderApp.InquiryPaymentTransaction(orderId, transactionId);
				return HandleOutput(appResponse);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		///   گزارش گیری از دستور پرداخت با جزییات تراکنش ها
		/// </summary>
		/// <param name="orderId"></param>
		/// <returns></returns>
		[HttpGet("{orderId}/report")]
		[ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(GroupedOrderReportDto))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
		public async Task<IActionResult> InquiryFromBank(string orderId)
		{
			try
			{
				var appResponse = await _groupedOrderApp.ReportOrderAsync(orderId);
				return HandleOutput(appResponse);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}


		/// <summary>
		///   گزارش گیری از تراکنش دستور پرداخت
		/// </summary>
		/// <param name="orderId"></param>
		/// <param name="transactionOrderId"></param>
		/// <returns></returns>
		[HttpGet("{orderId}/transaction/{transactionOrderId}/report")]
		[ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(GroupedOrderReportDto))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
		public async Task<IActionResult> InquiryFromBank(string orderId, string transactionOrderId)
		{
			try
			{
				var appResponse = await _groupedOrderApp.ReportTrasnactionAsync(orderId, transactionOrderId);
				return HandleOutput(appResponse);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
