public delegate void CzfEvent();
public delegate void CzfEvent<T>(T target);
public delegate void CzfEvent<T1, T2>(T1 target, T2 msg = default(T2));
public delegate void CzfEvent<T1, T2, T3>(T1 target, T2 msg = default(T2), T3 msg1 = default(T3));
public delegate void CzfEvent<T1, T2, T3, T4>(T1 target, T2 msg = default(T2), T3 msg1 = default(T3), T4 msg2 = default(T4));
public delegate void CzfEvent<T1, T2, T3, T4, T5>(T1 target, T2 msg = default(T2), T3 msg1 = default(T3), T4 msg2 = default(T4), T5 msg3 = default(T5));

public delegate R CzfFun<R>();
public delegate R CzfFun<T, R>(T target);
public delegate R CzfFun<T1, T2, R>(T1 target, T2 msg = default(T2));