import { useState, useEffect, useCallback } from 'react'
import { fetchWeather } from './services/api'
import CurrentWeather from './components/CurrentWeather'
import HourlyForecast from './components/HourlyForecast'
import DailyForecast from './components/DailyForecast'
import Loader from './components/Loader'
import ErrorBlock from './components/ErrorBlock'
import SnakeGame from './components/SnakeGame'
import ThemeToggle from './components/ThemeToggle'

function App() {
  const [data, setData] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)
  const [showSnake, setShowSnake] = useState(false)

  const loadWeather = useCallback(async (signal) => {
    setLoading(true)
    setError(null)
    try {
      const result = await fetchWeather(signal)
      setData(result)
      setLoading(false)
    } catch (e) {
      if (e.name === 'AbortError') return
      setError(e.message)
      setLoading(false)
    }
  }, [])

  useEffect(() => {
    const ac = new AbortController()
    loadWeather(ac.signal)
    return () => ac.abort()
  }, [loadWeather])

  if (loading) return <Loader />
  if (error) return <ErrorBlock message={error} onRetry={() => loadWeather()} />

  return (
    <div className="max-w-3xl mx-auto px-4 py-8 animate-in">
      {/* Header */}
      <div className="flex items-center justify-between mb-8">
        <div className="w-32" />
        <div className="text-center">
          <h1
            onClick={() => setShowSnake(s => !s)}
            className="text-3xl font-bold cursor-pointer transition-all duration-300 hover:scale-105 inline-block"
            style={{ color: 'var(--accent)', textShadow: `0 0 30px var(--accent-glow)` }}
            title="Нажми, чтобы сыграть в змейку!"
          >
            {data.city}
          </h1>
          <p className="text-secondary text-sm mt-1 tracking-wide">Прогноз погоды</p>
        </div>
        <div className="w-32 flex justify-end">
          <ThemeToggle />
        </div>
      </div>

      {showSnake && (
        <div className="animate-in">
          <SnakeGame onClose={() => setShowSnake(false)} />
        </div>
      )}

      <CurrentWeather data={data.current} />
      <HourlyForecast hours={data.hourly} />
      <DailyForecast days={data.daily} />
    </div>
  )
}

export default App
