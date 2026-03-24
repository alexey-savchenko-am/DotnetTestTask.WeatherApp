function CurrentWeather({ data }) {
  return (
    <div className="card rounded-3xl p-8 mb-6 text-center animate-in" style={{ animationDelay: '0.1s' }}>
      <div className="flex items-center justify-center gap-4 mb-3">
        <img src={data.icon} alt={data.description} className="w-20 h-20 drop-shadow-xl" />
        <span className="text-7xl font-extralight leading-none tracking-tighter">{Math.round(data.temp)}°</span>
      </div>
      <div className="text-xl font-light mb-1">{data.description}</div>
      <div className="text-secondary text-sm mb-6">Ощущается как {Math.round(data.feelsLike)}°</div>

      <div className="grid grid-cols-3 gap-4 max-w-sm mx-auto">
        <div className="card rounded-2xl py-3 px-2 text-center">
          <div className="text-lg font-semibold">{data.windKph}</div>
          <div className="text-secondary text-xs mt-0.5">км/ч · {data.windDir}</div>
        </div>
        <div className="card rounded-2xl py-3 px-2 text-center">
          <div className="text-lg font-semibold">{data.humidity}%</div>
          <div className="text-secondary text-xs mt-0.5">влажность</div>
        </div>
        <div className="card rounded-2xl py-3 px-2 text-center">
          <div className="text-lg font-semibold">{Math.round(data.pressure)}</div>
          <div className="text-secondary text-xs mt-0.5">мбар</div>
        </div>
      </div>
    </div>
  )
}

export default CurrentWeather
