namespace Airhockey.Interaction {
    public interface IInteract {
        public void OnHoverEnter();

        public void OnHoverExit();

        public void OnInteract(int button);
    }
}