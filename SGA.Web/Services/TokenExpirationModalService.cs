using Microsoft.AspNetCore.Components;

namespace SGA.Web.Services
{
    public interface ITokenExpirationModalService
    {
        event Action<bool>? OnShowModalChanged;
        void ShowModal();
        void HideModal();
        bool IsModalVisible { get; }
    }

    public class TokenExpirationModalService : ITokenExpirationModalService
    {
        private bool _isModalVisible = false;

        public event Action<bool>? OnShowModalChanged;

        public bool IsModalVisible => _isModalVisible;

        public void ShowModal()
        {
            if (!_isModalVisible)
            {
                _isModalVisible = true;
                OnShowModalChanged?.Invoke(_isModalVisible);
            }
        }

        public void HideModal()
        {
            if (_isModalVisible)
            {
                _isModalVisible = false;
                OnShowModalChanged?.Invoke(_isModalVisible);
            }
        }
    }
}
