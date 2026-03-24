const DAYS = ['Вс', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб']

function formatDay(dateStr) {
  const d = new Date(dateStr + 'T00:00:00')
  const today = new Date()
  if (d.toDateString() === today.toDateString()) return 'Сегодня'

  const tomorrow = new Date()
  tomorrow.setDate(tomorrow.getDate() + 1)
  if (d.toDateString() === tomorrow.toDateString()) return 'Завтра'

  return DAYS[d.getDay()] + ', ' + d.getDate()
}

function DailyForecast({ days }) {
  if (!days || days.length === 0) return null

  return (
    <div className="animate-in" style={{ animationDelay: '0.3s' }}>
      <div className="text-sm font-semibold uppercase tracking-widest text-secondary mb-3">
        3 дня
      </div>
      <div className="flex flex-col gap-2">
        {days.map((d) => (
          <div className="card flex items-center rounded-2xl px-5 py-4 gap-4" key={d.date}>
            <div className="w-20 text-sm font-semibold">{formatDay(d.date)}</div>
            <img src={d.icon} alt={d.description || 'погода'} width={36} height={36} className="drop-shadow" />
            <div className="flex-1 text-sm text-secondary">{d.description}</div>
            <div className="text-right">
              <span className="text-base font-bold">{Math.round(d.maxTemp)}°</span>
              <span className="text-secondary text-sm ml-1">{Math.round(d.minTemp)}°</span>
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}

export default DailyForecast
