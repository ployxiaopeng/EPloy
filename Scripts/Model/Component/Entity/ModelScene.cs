namespace ETModel
{
    public sealed class ModelScene : Entity
	{
		public string Name { get; set; }

		public ModelScene ()
		{
		}

		public ModelScene (long id): base(id)
		{
		}

		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}

			base.Dispose();
		}
	}
}