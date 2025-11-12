using System.Collections.Generic;

namespace CameraBehaviour.DataLayer.Config.Validate
{
    public interface IValidatable
    {
        // 에디터에서 유효성 점검 메시지 수집
        void Validate(List<string> warnings);
    }
}