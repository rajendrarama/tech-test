using FluentValidation;
using Order.Model;
using System;
using System.Linq;

namespace Order.WebAPI.Validators
{
    public class OrderDetailValidator : AbstractValidator<OrderDetail>
    {
        public OrderDetailValidator()
        {
            RuleFor(o => o.CustomerId).SetValidator(new GuidValidator());
            RuleFor(o => o.ResellerId).SetValidator(new GuidValidator());
            RuleFor(o => o.StatusId).SetValidator(new GuidValidator());
            RuleFor(o => o.StatusName).NotEmpty();
            RuleFor(o => o.Items).Must(x => x.Count() > 0);
        }
    }
    public class GuidValidator : AbstractValidator<Guid>
    {
        public GuidValidator()
        {
            RuleFor(x => x).NotEmpty();
        }
    }
}
