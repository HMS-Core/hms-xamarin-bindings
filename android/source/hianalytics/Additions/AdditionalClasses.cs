using Android.Database.Sqlite;
using Org.Greenrobot.Greendao.Database;

namespace Huawei.Hms.Analytics.Database
{
    partial class APIEventDao : Org.Greenrobot.Greendao.AbstractDao
    {
         public APIEventDao() : base(null)
        {
        }
        protected override void BindValues(SQLiteStatement p0, Java.Lang.Object p1)
        {
            BindValues(p0, p1);
        }

        protected override void BindValues(IDatabaseStatement p0, Java.Lang.Object p1)
        {
            BindValues(p0, p1);
        }

        protected override Java.Lang.Object GetKey(Java.Lang.Object p0)
        {
            return GetKey(p0);
        }

        protected override bool HasKey(Java.Lang.Object p0)
        {
            return HasKey(p0);
        }

        protected override Java.Lang.Object ReadEntity(global::Android.Database.ICursor p0, int p1)
        {
            return ReadEntity(p0, p1);
        }

        protected override void ReadEntity(global::Android.Database.ICursor p0, Java.Lang.Object p1, int p2)
        {
            ReadEntity(p0, p1, p2);
        }

        protected override Java.Lang.Object ReadKey(global::Android.Database.ICursor p0, int p1)
        {
            return ReadKey(p0, p1);
        }

        protected override Java.Lang.Object UpdateKeyAfterInsert(Java.Lang.Object p0, long p1)
        {
            return UpdateKeyAfterInsert(p0, p1);
        }
    }

    partial class EventDao : Org.Greenrobot.Greendao.AbstractDao
    {
        public EventDao() : base(null)
        {
        }
        //protected override bool IsEntityUpdateable => IsEntityUpdateable;
        protected override void BindValues(SQLiteStatement p0, Java.Lang.Object p1)
        {
            BindValues(p0, p1);
        }

        protected override void BindValues(IDatabaseStatement p0, Java.Lang.Object p1)
        {
            BindValues(p0, p1);
        }

        protected override Java.Lang.Object GetKey(Java.Lang.Object p0)
        {
            return GetKey(p0);
        }

        protected override bool HasKey(Java.Lang.Object p0)
        {
            return HasKey(p0);
        }

        protected override Java.Lang.Object ReadEntity(global::Android.Database.ICursor p0, int p1)
        {
            return ReadEntity(p0, p1);
        }

        protected override void ReadEntity(global::Android.Database.ICursor p0, Java.Lang.Object p1, int p2)
        {
            ReadEntity(p0, p1, p2);
        }

        protected override Java.Lang.Object ReadKey(global::Android.Database.ICursor p0, int p1)
        {
            return ReadKey(p0, p1);
        }

        protected override Java.Lang.Object UpdateKeyAfterInsert(Java.Lang.Object p0, long p1)
        {
            return UpdateKeyAfterInsert(p0, p1);
        }
    }
}