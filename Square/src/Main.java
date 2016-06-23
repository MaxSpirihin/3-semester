import java.util.Scanner;

import javax.swing.JFrame;
import javax.swing.SwingUtilities;

public class Main {

	public static final int START_X = 20;
	public static final int START_Y = 20;
	public static final int PIXELS_IN_COORDINATE = 5;



	public static void main(String[] arg) {

		Rectangles rectangles = null;

		@SuppressWarnings("resource")
		Scanner in = new Scanner(System.in);
		System.out
				.println("Enter count of rectangles. For start test mode enter 0");
		try {
			int count = in.nextInt();
			if (count == 0) {
				System.out.println("Enter number of test 1-4");
				int test = in.nextInt();
				rectangles = Tester.GetTestInstanceOfRectangles(test);
			} else {
				rectangles = new Rectangles(count);
				
				for (int i=0;i<count;i++)
				{
					System.out.println("Enter width and size of " + String.valueOf(i+1) + " rectangle.");
					rectangles.get(i).SetSize(in.nextInt(), in.nextInt());
				}

			}

			boolean result = rectangles.BuildSquare();

			final MyPanel panel = new MyPanel(rectangles, result);

			SwingUtilities.invokeLater(new Runnable() {

				public void run() {
					createAndShowGUI(panel);
				}
			});

		} catch (Exception ex) {
			System.out.println("Wrong data");
		}

	}



	private static void createAndShowGUI(MyPanel panel) {
		JFrame f = new JFrame("Square from rectangles");
		f.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		f.add(panel);
		f.pack();
		f.setVisible(true);
	}

}