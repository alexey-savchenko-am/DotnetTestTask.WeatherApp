import { useState, useEffect } from 'react'

const THEMES = [
  { key: 'system', icon: '◐', label: 'Авто' },
  { key: 'light', icon: '☀', label: 'Светлая' },
  { key: 'dark', icon: '☾', label: 'Тёмная' },
]

function getSystemTheme() {
  return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'
}

function applyTheme(theme) {
  const resolved = theme === 'system' ? getSystemTheme() : theme
  document.documentElement.dataset.theme = resolved
}

function ThemeToggle() {
  const [theme, setTheme] = useState(() => {
    return localStorage.getItem('weather-theme') || 'system'
  })

  useEffect(() => {
    applyTheme(theme)
    localStorage.setItem('weather-theme', theme)
  }, [theme])

  useEffect(() => {
    if (theme !== 'system') return
    const mq = window.matchMedia('(prefers-color-scheme: dark)')
    const handler = () => applyTheme('system')
    mq.addEventListener('change', handler)
    return () => mq.removeEventListener('change', handler)
  }, [theme])

  return (
    <div
      className="flex items-center gap-0.5 rounded-full p-1"
      style={{ background: 'var(--toggle-bg)' }}
    >
      {THEMES.map(({ key, icon }) => (
        <button
          key={key}
          onClick={() => setTheme(key)}
          title={THEMES.find(t => t.key === key).label}
          className="w-8 h-8 rounded-full border-none cursor-pointer transition-all duration-200 text-sm flex items-center justify-center"
          style={{
            background: theme === key ? 'var(--toggle-active)' : 'transparent',
            color: theme === key ? 'var(--toggle-text-active)' : 'var(--toggle-text)',
          }}
        >
          {icon}
        </button>
      ))}
    </div>
  )
}

export default ThemeToggle
