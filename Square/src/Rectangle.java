import java.awt.Graphics;

//наш прямоугольник
public class Rectangle {

	private int width;
	private int height;

	// координаты левого верхнего угла относительно верхнего левого угла
	// квадрата
	private int x;
	private int y;
	
	//показывает положили ли его в наш квадрат
	private boolean isInSquare;



	public Rectangle(int width, int height) {
		this.width = width;
		this.height = height;
		x = 0;
		y = 0;
		isInSquare = false;
	}
	
	public boolean IsInSquare()
	{
		return isInSquare;
	}
	
	public void PutToSquare()
	{
		isInSquare = true;
	}
	
	public void DeleteFromSquare()
	{
		isInSquare = false;
	}



	public int getWidth() {
		return width;
	}



	public int getHeight() {
		return height;
	}



	public int getX() {
		return x;
	}



	public int getY() {
		return y;
	}



	public void SetCoordinates(int x, int y) {
		this.x = x;
		this.y = y;
	}



	public void SetSize(int width, int height) {
		this.width = width;
		this.height = height;
	}
	



	public void Print() {
		System.out.print("Width = " + String.valueOf(width) + "; ");
		System.out.print("Height = " + String.valueOf(height) + "; ");
		System.out.print("X = " + String.valueOf(x) + "; ");
		System.out.println("Y = " + String.valueOf(y) + "; ");
	}



	public void Draw(Graphics g) {
		g.drawLine((x + Main.START_X) * Main.PIXELS_IN_COORDINATE,
				(y + Main.START_Y) * Main.PIXELS_IN_COORDINATE,
				(x + Main.START_X) * Main.PIXELS_IN_COORDINATE, (y
						+ Main.START_Y + height)
						* Main.PIXELS_IN_COORDINATE);
		g.drawLine((x + Main.START_X) * Main.PIXELS_IN_COORDINATE,
				(y + Main.START_Y) * Main.PIXELS_IN_COORDINATE, (x
						+ Main.START_X + width)
						* Main.PIXELS_IN_COORDINATE, (y + Main.START_Y)
						* Main.PIXELS_IN_COORDINATE);
		g.drawLine((x + Main.START_X + width) * Main.PIXELS_IN_COORDINATE,
				(y + Main.START_Y) * Main.PIXELS_IN_COORDINATE, (x
						+ Main.START_X + width)
						* Main.PIXELS_IN_COORDINATE,
				(y + Main.START_Y + height) * Main.PIXELS_IN_COORDINATE);
		g.drawLine((x + Main.START_X) * Main.PIXELS_IN_COORDINATE, (y
				+ Main.START_Y + height)
				* Main.PIXELS_IN_COORDINATE, (x + Main.START_X + width)
				* Main.PIXELS_IN_COORDINATE, (y + Main.START_Y + height)
				* Main.PIXELS_IN_COORDINATE);
	}

}
