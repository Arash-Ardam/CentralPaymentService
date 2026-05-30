using Application.OrderManagement;
using Application.OrderManagement.Dtos.SingleOrder;
using CentralPaymentWebApi.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentralPaymentWebApi.Controllers.Payment;

[Authorize(Policy = AuthorizationConsts.UserPolicy)]
public class SinglePaymentController : ApiControllerBase
{
	private readonly ISingleOrderApplication _singleOrderApp;

	public SinglePaymentController(ISingleOrderApplication singleOrderApplication)
	{
		_singleOrderApp = singleOrderApplication;
	}

	/// <summary>
	/// ایجاد تراکنش تکی جدید
	/// </summary>
	/// <param name="createDto"></param>
	/// <returns></returns>
	[HttpPost(RouteTemplates.Create)]
	[ProducesResponseType(StatusCodes.Status201Created,Type = typeof(string))]
	[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
	[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
	[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
	public async Task<IActionResult> CreateAsync(CreateSingleOrderDto createDto)
	{
		try
		{
			var appResponse = await _singleOrderApp.CreateAsync(createDto);
			return HandleOutput(appResponse);	
		}
		catch (Exception ex)
		{
			return BadRequest(ex.Message);
		}
	}

	/// <summary>
	/// افزودن جزییات تراکنش تکی
	/// </summary>
	/// <param name="transactionDto"></param>
	/// <returns></returns>
	[HttpPost("specs/add")]
	[ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(string))]
	[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
	[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
	[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
	public async Task<IActionResult> AddTransactionAsync(SingleTransactionDto transactionDto)
	{
		try
		{
			var appResponse = await _singleOrderApp.AddTransaction(transactionDto);
			return HandleOutput(appResponse);
		}
		catch (Exception ex)
		{
			return BadRequest(ex.Message);
		}
	}

	/// <summary>
	/// حذف جزییات تراکنش تکی
	/// </summary>
	/// <param name="orderId"></param>
	/// <returns></returns>
	[HttpDelete("transaction/{orderId}/remove")]
	[ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(string))]
	[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
	[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
	[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
	public async Task<IActionResult> RemoveTransaction(Guid orderId)
	{
		try
		{
			var appResponse = await _singleOrderApp.RemoveTransaction(orderId);
			return HandleOutput(appResponse);
		}
		catch (Exception ex)
		{
			return BadRequest(ex.Message);
		}
	}

	/// <summary>
	/// نهایی سازی تراکنش تکی
	/// </summary>
	/// <param name="orderId"></param>
	/// <returns></returns>
	[HttpPost("transaction/{orderId}/finalize")]
	[ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(string))]
	[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
	[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
	[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
	public async Task<IActionResult> FinalizeOrder(Guid orderId)
	{
		try
		{
			var appResponse = await _singleOrderApp.FinalizeOrder(orderId);
			return HandleOutput(appResponse);
		}
		catch (Exception ex)
		{
			return BadRequest(ex.Message);
		}
	}

	/// <summary>
	///  ارسال تراکنش تکی به بانک جهت پردازش
	/// </summary>
	/// <param name="orderId"></param>
	/// <returns></returns>
	[HttpPost("transaction/{orderId}/send")]
	[ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(string))]
	[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
	[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
	[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
	public async Task<IActionResult> SendToBank(Guid orderId)
	{
		try
		{
			var appResponse = await _singleOrderApp.SendOrderAsync(orderId);
			return HandleOutput(appResponse);
		}
		catch (Exception ex)
		{
			return BadRequest(ex.Message);
		}
	}

	/// <summary>
	///  استعلام وضعیت تراکنش تکی از بانک 
	/// </summary>
	/// <param name="orderId"></param>
	/// <returns></returns>
	[HttpPost("transaction/{orderId}/inquiry")]
	[ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(string))]
	[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
	[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
	[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
	public async Task<IActionResult> InquiryFromBank(Guid orderId)
	{
		try
		{
			var appResponse = await _singleOrderApp.InquiryPaymentOrder(orderId);
			return HandleOutput(appResponse);
		}
		catch (Exception ex)
		{
			return BadRequest(ex.Message);
		}
	}
}