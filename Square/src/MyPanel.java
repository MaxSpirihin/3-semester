import java.awt.Color;
import java.awt.Dimension;
import java.awt.Graphics;

import javax.swing.BorderFactory;
import javax.swing.JPanel;

//панель вывода, для отрисовки
@SuppressWarnings("serial")
class MyPanel extends JPanel {

	Rectangles rect;
	boolean result;



	public MyPanel(Rectangles rectangles, boolean result) {
		this.result = result;
		rect = rectangles;
		setBorder(BorderFactory.createLineBorder(Color.black));
	}



	public Dimension getPreferredSize() {
		return new Dimension(640, 480);
	}



	public void paintComponent(Graphics g) {
		super.paintComponent(g);
		if (result) {
			rect.DrawAll(g);
		} else {
			g.drawString("It is impossible", Main.START_X, Main.START_Y);
		}
	}
}