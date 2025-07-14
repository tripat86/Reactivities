import { Divider, Paper, Typography } from "@mui/material";
import { useLocation } from "react-router";

/*
Great question!

In the line:

```ts
router.navigate('/server-error', { state: { error: data } });
```

the `state` is a **way to pass data to the destination route** in React Router **without putting it in the URL**.

---

### 🧠 What is `state` in `router.navigate`?

* It’s an **optional** object you can pass when navigating programmatically.
* The data in `state` is available to the **target component** via `useLocation()`.

---

### ✅ Example in Detail

#### ✅ 1. **Navigate with state:**

```ts
router.navigate('/server-error', { state: { error: data } });
```

Here, `data` could be an error object, validation messages, or anything else.

---

#### ✅ 2. **Access in `/server-error` route:**

```tsx
import { useLocation } from 'react-router-dom';

export default function ServerErrorPage() {
  const location = useLocation();
  const error = location.state?.error;

  return (
    <div>
      <h1>Server Error</h1>
      {error && <pre>{JSON.stringify(error, null, 2)}</pre>}
    </div>
  );
}
```

---

### 🟨 Notes:

* The data **does not persist** across refreshes — it's stored **in memory**, not in the URL or localStorage.
* If you refresh `/server-error`, `location.state` will be `undefined`.

---

### 🆚 Compare with URL Params:

| Feature          | URL Params (`/error/:code`) | State (`{ state: { error } }`)    |
| ---------------- | --------------------------- | --------------------------------- |
| Visible in URL   | ✅ Yes                       | ❌ No                              |
| Good for Sharing | ✅ Yes                       | ❌ No                              |
| Supports Objects | ❌ No (must be strings)      | ✅ Yes                             |
| Lost on Refresh  | ❌ No                        | ✅ Yes (unless persisted manually) |

---

Let me know if you want to persist this error in the URL or localStorage as a fallback!

*/
export default function ServerError() {
    const { state } = useLocation();
    return (
        <Paper>
            {
                state.error ? (
                    <>
                        <Typography gutterBottom variant="h3" sx={{ px: 4, pt: 2 }}>
                            {/* If the error has a message property, display it
                             Otherwise, display a generic message*/}
                            {state.error?.message || 'There has been a error'}
                        </Typography>
                        <Divider />
                        <Typography variant="body1" sx={{ px: 4 }}>
                             {/* If the error has a details property, display it
                             Otherwise, display a generic message */}
                            {state.error?.details || 'Internal server error'}
                        </Typography>

                        <pre>{state.error.detail}</pre>
                    </>
                ) : (
                    <Typography variant="h5">
                        Server error
                    </Typography>
                )
            }
        </Paper>
    )
}