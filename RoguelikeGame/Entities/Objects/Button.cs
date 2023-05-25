using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RoguelikeGame.Entities.Objects;

public class Button : IEntity, ISolid
{
    public int ImageId { get; }
    public Vector2 Position { get; set; }
    public Vector2 Speed { get; set; }
    public RectangleCollider Collider { get; set; }

    private bool _isClicked;
    public readonly string Text;
    public readonly Color TextColor;

    public Button(int imageId, Vector2 position, int buttonWidth, int buttonHeight, string text,
        Color textColor)
    {
        ImageId = imageId;
        Position = position;
        Text = text;
        TextColor = textColor;
        
        Collider = new RectangleCollider((int)position.X, (int)position.Y, buttonWidth, buttonHeight);
    }

    public void Update() {}
    public void Update(MouseState mouseState)
    {
        var mouseRectangle = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
        
        if (mouseRectangle.Intersects(Collider.Boundary))
            _isClicked = mouseState.LeftButton == ButtonState.Pressed;
        else
            _isClicked = false;
    }
    
    public bool IsClicked() => _isClicked;
    public void Clicked() => _isClicked = false;

    public void MoveCollider(Vector2 newPos) { }
}