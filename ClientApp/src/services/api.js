export async function fetchWeather(signal) {
  const resp = await fetch('/api/weather', { signal });

  if (!resp.ok) {
    const body = await resp.json().catch(() => null);
    throw new Error(body?.error || 'Ошибка загрузки');
  }

  return resp.json();
}
