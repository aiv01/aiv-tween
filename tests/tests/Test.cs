using NUnit.Framework;
using System;
using Aiv.Tween;
using System.Collections.Generic;
using System.Reflection;

namespace tests
{
	[TestFixture ()]
	public class Test
	{

		// custom class for testing properties and fields
		public class FooBar
		{
			public float counter;
			private float _hidden;

			public float Hidden {
				get {
					return _hidden;
				}
				set {
					_hidden = value;
				}

			}
		}

		public float genericValue;

		[Test ()]
		public void TestInitialization ()
		{
			Tween tween = new Tween ();
			Assert.AreEqual (tween.Now, 0);
			Assert.AreEqual (tween.Round, 0);
			Assert.AreEqual (tween.Gradient, 0);
		}

		[Test ()]
		[ExpectedException (typeof(Exception))]
		public void TestRedStart ()
		{
			Tween tween = new Tween ();
			tween.Start ();
		}

		[Test ()]
		public void TestGreenStart ()
		{
			Tween tween = new Tween ();
			tween.Call (t => t.Repeat (1));
			tween.Start ();
		}

		[Test ()]
		public void TestUpdate ()
		{

			FooBar fooBar = new FooBar ();
			fooBar.counter = 0;

			Tween tween = new Tween ();
			tween.To (fooBar, new {counter = 17}, 1).Start ();
			tween.Update (0);
			Assert.AreEqual (fooBar.counter, 0);
			tween.Update (1);
			Assert.AreEqual (fooBar.counter, 17);
		}

		[Test ()]
		public void TestUpdateWithProperty ()
		{

			FooBar fooBar = new FooBar{ Hidden = 30 };

			Tween tween = new Tween ();
			tween.To (fooBar, new {Hidden = 17}, 1).Start ();
			tween.Update (0).Update (1);
			Assert.AreEqual (fooBar.Hidden, 17);
		}

		[Test ()]
		public void TestUpdateWithPropertyHalf ()
		{

			FooBar fooBar = new FooBar{ Hidden = 30 };

			Tween tween = new Tween ();
			tween.To (fooBar, new {Hidden = 17}, 2).Start ();
			tween.Update (0).Update (1);
			Assert.AreEqual (fooBar.Hidden, 23.5f);
		}

		[Test ()]
		public void TestCall ()
		{

			bool fake = false;
			Tween tween = new Tween ().Call (t => fake = true);
			tween.Start ();
			tween.Update (0);
			Assert.IsTrue (fake);
		}

		[Test ()]
		public void TestCallLoop ()
		{

			int counter = 0;
			Tween tween = new Tween ().Call (t => counter++);
			tween.Loop ().Start ();
			tween.Update (0).Update (1).Update (2);
			Assert.AreEqual (counter, 3);
		}

		[Test ()]
		public void TestRedCallLoop ()
		{

			int counter = 0;
			Tween tween = new Tween ().Call (t => counter++);
			tween.Loop ().Start ();
			tween.Update (0).Update (1).Update (2).Update (100);
			Assert.AreNotEqual (counter, 3);
		}

		[Test ()]
		public void TestCallRepeat ()
		{

			int counter = 0;
			Tween tween = new Tween ().Call (t => counter++);
			tween.Repeat (2).Start ();
			tween.Update (0).Update (1).Update (2);
			Assert.AreEqual (counter, 2);
		}

		[Test ()]
		public void TestRound ()
		{

			int counter = 0;
			Tween tween = new Tween ().Call (t => counter++);
			tween.Repeat (2).Start ();
			tween.Update (0).Update (1).Update (2);
			Assert.AreEqual (tween.Round, 2);
		}

		[Test ()]
		public void TestField ()
		{

			this.genericValue = 17;
			Tween tween = new Tween ();
			tween.To (this, new {genericValue = 30}, 1);
			tween.Start ();
			tween.Update (0).Update (1);
			Assert.AreEqual (this.genericValue, 30);
		}

		[Test ()]
		public void TestGradient ()
		{
			this.genericValue = 0;
			Tween tween = new Tween ().To (this, new {genericValue = 1}, 1);
			tween.Start ().Update (0).Update (0.5f);
			Assert.AreEqual (tween.Gradient, 0.5f);
		}

		[Test ()]
		public void TestKeyFrameIndex ()
		{
			this.genericValue = 0;
			Tween tween = new Tween ().To (this, new {genericValue = 1}, 1);
			tween.To (this, new {genericValue = 10}, 2);
			tween.Start ().Update (0).Update (1.5f);
			Assert.AreEqual (tween.CurrentKeyFrameIndex, 1);
		}

		[Test ()]
		public void TestRedKeyFrameIndex ()
		{
			this.genericValue = 0;
			Tween tween = new Tween ().To (this, new {genericValue = 1}, 1);
			tween.To (this, new {genericValue = 10}, 2);
			tween.Start ().Update (0).Update (0.5f);
			Assert.AreNotEqual (tween.CurrentKeyFrameIndex, 2);
		}

		[Test ()]
		public void TestUpdateEvent ()
		{
			int counter = 0;
			Tween tween = new Tween ().Delay (100);
			tween.OnUpdate += (t => counter++);
			tween.Start ();

			tween.Update (0).Update (1).Update (2);

			Assert.AreEqual (counter, 3);
		}

		[Test ()]
		public void TestDelay ()
		{
			Tween tween = new Tween ().Delay (100);
			tween.Start ();

			tween.Update (0).Update (101);

			Assert.IsFalse (tween.IsPlaying);
		}

		[Test ()]
		public void TestRedDelay ()
		{
			Tween tween = new Tween ().Delay (100);
			tween.Start ();

			tween.Update (0).Update (99.99999f);

			Assert.IsTrue (tween.IsPlaying);
		}

		[Test ()]
		public void TestIsPlaying ()
		{
			Tween tween = new Tween ().Delay (100);
			Assert.IsFalse (tween.IsPlaying);
			tween.Start ();
			Assert.IsTrue (tween.IsPlaying);
		}

		[Test ()]
		public void TestNow ()
		{
			float now = 0;
			Tween tween = new Tween ().Call (t => now = t.Now).Start ();
			tween.Update (173);
			Assert.AreEqual (now, 173);
		}

		[Test ()]
		public void TestKeyFramesCount ()
		{
			Tween tween = new Tween ();
			Assert.AreEqual (tween.KeyFramesCount, 0);
			tween.Nop ().Nop ();
			Assert.AreEqual (tween.KeyFramesCount, 2);
		}

		[Test ()]
		public void TestMultiObjects ()
		{
			this.genericValue = 0;
			FooBar fooBar = new FooBar ();
			Tween tween = new Tween ();
			tween.To (this, new {genericValue = 1}, fooBar, new {counter = 17, Hidden = 30}, 2);
			tween.Start ().Update (0).Update (1);
			Assert.AreEqual (this.genericValue, 1f / 2);
			Assert.AreEqual (fooBar.Hidden, 30f / 2);
			Assert.AreEqual (fooBar.counter, 17f / 2);

		}

		[Test ()]
		public void TestEasing ()
		{
			Tween tween = new Tween ().SetEasing (k => 0);
			tween.To (this, new {genericValue = 173}, 1).Start ();
			tween.Update (0).Update (100).Update (200);
			Assert.AreEqual (this.genericValue, 0);

		}

		[Test ()]
		public void TestMultiTo ()
		{
			this.genericValue = 0;
			Tween tween = new Tween ();
			tween.To (this, new {genericValue = 173}, 1);
			tween.To (this, new {genericValue = 26}, 1);
			tween.Start ();
			tween.Update (0);
			Assert.AreEqual (tween.CurrentKeyFrameIndex, 0);
			Assert.AreEqual (this.genericValue, 0);
			tween.Update (1);
			Assert.AreEqual (tween.CurrentKeyFrameIndex, 1);
			Assert.AreEqual (this.genericValue, 173);
			tween.Update (2);
			Assert.AreEqual (tween.CurrentKeyFrameIndex, 1);
			Assert.AreEqual (this.genericValue, 26);

		}

		[Test ()]
		public void TestMultiEasing ()
		{
			Tween tween = new Tween ();
			tween.SetEasing (k => 1).To (this, new {genericValue = 173}, 1);
			tween.SetEasing (k => 2).To (this, new {genericValue = 250}, 1);
			tween.Start ().Update (0);
			Assert.AreEqual (this.genericValue, 173);
			Assert.AreEqual (tween.CurrentKeyFrameIndex, 0);
			tween.Update (0.5f);
			Assert.AreEqual (this.genericValue, 173);
			Assert.AreEqual (tween.CurrentKeyFrameIndex, 0);
			tween.Update (1);
			Assert.AreEqual (this.genericValue, 173);
			Assert.AreEqual (tween.CurrentKeyFrameIndex, 1);
			tween.Update (2);
			Assert.AreEqual (this.genericValue, 327);
		}

		[Test ()]
		public void TestRedNow ()
		{
			Tween tween = new Tween ().Nop ().Start ();
			tween.Update (0.5f);
			Assert.AreNotEqual (tween.Now, 0.1f);

		}
	}
}

