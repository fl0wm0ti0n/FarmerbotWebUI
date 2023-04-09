
namespace FarmerbotWebUI.Shared
{
    public static class ResultMapper
    {
        public static bool ToBool(MessageResult messageResult)
        {
            return messageResult switch
            {
                MessageResult.Successfully => true,
                MessageResult.Unsuccessfully => false,
                MessageResult.Valueless => throw new NotImplementedException(),
                MessageResult.Unknown => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };
        }

        public static MessageResult ToProgressBarStyle(bool result)
        {
            return result switch
            {
                true => MessageResult.Successfully,
                false => MessageResult.Unsuccessfully,

            };
        }
    }
}
