/******************************************************************************
 * Generic Data Structures
 *****************************************************************************/

public class Node<T>
{
	public T data;
	public Node<T>? prev, next;

	public Node(T data) {
		this.data = data;
		prev = null;
		next = null;
	}
}

public class RingBuffer<T>
{
	private Node<T>? head;
	private Node<T>? tail;
	private Node<T>? cur;
	private int size;

	public RingBuffer()
	{
		head = cur = null;
		size = 0;
	}

	// get number of elements
	public int getSize()
	{
		return size;
	}


	// add a new element
	public void add(T data)
	{
		if (head == null || tail == null || cur == null)
		{
			head = tail = cur = new Node<T>(data);
		} else {
			Node<T> n = new Node<T>(data);
			tail.next = n;
			n.prev = tail;
			tail = tail.next;
		}

		size++;
	}


	// get the current element
	public T getCur()
	{
		return cur.data;
	}


	// switch to the next element (round-robin)
	public void next()
	{
		if (cur == null)
			return;

		cur = cur.next;
		if (cur == null)
			cur = head;
	}

	// switch to the previous element (round-robin)
	public void prev()
	{
		if (cur == null)
			return;

		cur = cur.prev;
		if (cur == null)
			cur = tail;
	}

	// switch to element with index (round-robin)
	public void select(int idx)
	{
		cur = head;
		for (int i = 0; i<idx ; i++)
			next();
	}
}



/******************************************************************************
 * Demo specific Datastructures and Data
 *****************************************************************************/

public enum TimeOfDay { Day, Night };
public enum Weather { Sun, Rain };

public class ArousalValence
{
	public double a; /* -1.0 .. 1.0 */
	public double v; /* -1.0 .. 1.0 */

	public ArousalValence(double a, double v)
	{
		this.a = a;
		this.v = v;
	}

	public double getAbsVectorValue()
	{
		return Math.Sqrt(a*a+v*v);
	}

	public override string ToString()
	{
		return "(arousal=" + a + ", valence=" + v +
			", abs=" + string.Format("{0:N2}", getAbsVectorValue()) +
			")";
	}
}


public class DemoDataElem
{
	public int id;
	public DayOfWeek dayOfWeek;
	public TimeOfDay timeOfDay;
	public Weather weather;
	public ArousalValence context; /* dayOfWeek, timeOfDay, weather, news */
	public ArousalValence direct; /* DM, social media */
	public int nTouches;

	public DemoDataElem(
			int id,
			DayOfWeek dayOfWeek, TimeOfDay timeOfDay, Weather weather,
			ArousalValence context, ArousalValence direct,
			int nTouches)
	{
		this.id = id;
		this.dayOfWeek = dayOfWeek;
		this.timeOfDay = timeOfDay;
		this.weather = weather;
		this.context = context;
		this.direct = direct;
		this.nTouches = nTouches;
	}

	public ArousalValence getContext()
	{
		return context;
	}

	public ArousalValence getDirect()
	{
		return direct;
	}

	public override string ToString()
	{
		return "(" +
			"id=" + id +
			" dayOfWeek=" + dayOfWeek +
			" timeOfDay=" + timeOfDay +
			" weather=" + weather +
			" context=" + getContext() +
			" direct=" + getDirect() +
			" nTouches=" + nTouches +
			")";
	}

}

public class DemoData : RingBuffer<DemoDataElem>
{
	// data factory

	public static DemoData create()
	{
		DemoData dd = new DemoData();

		/*                      id    Day of Week           Time of Day      Weather       Context         (  a     v )     Direct          (  a     v )   #Touches   */
		dd.add(new DemoDataElem(0,  DayOfWeek.Monday,     TimeOfDay.Day,   Weather.Sun,  new ArousalValence( 0.0,  0.0),  new ArousalValence( 0.0,  0.0),      0     )); /* neutral/disabled */

		dd.add(new DemoDataElem(1,  DayOfWeek.Monday,     TimeOfDay.Day,   Weather.Rain, new ArousalValence(-0.2, -0.9),  new ArousalValence( 0.2,  0.2),      0     ));
		dd.add(new DemoDataElem(2,  DayOfWeek.Sunday,     TimeOfDay.Night, Weather.Sun,  new ArousalValence(-0.3,  0.5),  new ArousalValence(-0.2, -0.8),      0     ));
		dd.add(new DemoDataElem(3,  DayOfWeek.Saturday,   TimeOfDay.Day,   Weather.Sun,  new ArousalValence( 0.8,  0.8),  new ArousalValence( 0.8, -0.8),     25     ));
		dd.add(new DemoDataElem(4,  DayOfWeek.Wednesday,  TimeOfDay.Day,   Weather.Sun,  new ArousalValence(-0.1,  0.1),  new ArousalValence( 0.1,  0.1),      2     ));
		dd.add(new DemoDataElem(5,  DayOfWeek.Friday,     TimeOfDay.Night, Weather.Sun,  new ArousalValence( 1.0,  1.0),  new ArousalValence( 1.0,  1.0),     10     ));

		return dd;
	}

	// small test routine
	public static void test()
	{
		DemoData dd = DemoData.create();

		Console.WriteLine("size {0:D}", dd.getSize());

		Console.WriteLine("access forwards");
		for (int i=0; i<dd.getSize()*2; i++) {
			for (int j=0; j<2; j++) {
				DemoDataElem d = dd.getCur();
				Console.WriteLine("access#{0:D}: {1}", j, d);
			}
			Console.WriteLine("select next");
			dd.next();
		}

		Console.WriteLine("access backwards");
		Console.WriteLine("select prev");
		dd.prev();
		for (int i=0; i<dd.getSize()*2; i++) {
			for (int j=0; j<2; j++) {
				DemoDataElem d = dd.getCur();
				Console.WriteLine("access#{0:D}: {1}", j, d);
			}
			Console.WriteLine("select prev");
			dd.prev();
		}

		Console.WriteLine("access selective");
		for (int i=0; i<dd.getSize()*2; i++) {
			Console.WriteLine("select {0:D}", i);
			dd.select(i);
			for (int j=0; j<2; j++) {
				DemoDataElem d = dd.getCur();
				Console.WriteLine("access#{0:D}: {1}", j, d);
			}
		}
	}
}
