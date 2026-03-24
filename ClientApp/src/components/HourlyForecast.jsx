function HourlyForecast({ hours }) {
  if (!hours || hours.length === 0) return null

  return (
    <div className="mb-6 animate-in" style={{ animationDelay: '0.2s' }}>
      <div className="text-sm font-semibold uppercase tracking-widest text-secondary mb-3">
        Почасовой
      </div>
      <div className="hourly-scroll flex gap-2 overflow-x-auto pb-2" style={{ WebkitOverflowScrolling: 'touch' }}>
        {hours.map((h, i) => (
          <div
            className="card shrink-0 rounded-2xl px-4 py-3 text-center min-w-[76px] cursor-default"
            key={`${i}-${h.time}`}
          >
            <div className="text-xs text-secondary mb-1.5">{h.time}</div>
            <img src={h.icon} alt={h.description || 'погода'} width={32} height={32} className="mx-auto drop-shadow" />
            <div className="text-sm font-bold mt-1.5">{Math.round(h.temp)}°</div>
          </div>
        ))}
      </div>
    </div>
  )
}

export default HourlyForecast
