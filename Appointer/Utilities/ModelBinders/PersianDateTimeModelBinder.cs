using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Appointer.Utilities.ModelBinders
{
    public class PersianDateTimeModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var modelState = new ModelState { Value = valueResult };
            object actualValue = null;
            try
            {
                if (valueResult.AttemptedValue.IsPersianDateTime() == false)
                {
                    var metadata = bindingContext.ModelMetadata;
                    var displayName = metadata.DisplayName ?? metadata.PropertyName ?? bindingContext.ModelName.Split('.').Last();
                    modelState.Errors.Add(string.Format("{0} را بدرستی وارد کنید", displayName));
                }
                else
                {
                    var datetime = Convert.ToDateTime(valueResult.AttemptedValue);
                    var miladi = datetime.ToMiladiDateTime();
                    actualValue = miladi;
                }
            }
            catch (FormatException e)
            {
                modelState.Errors.Add(e);
            }
            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
            return actualValue;
        }
    }
}