import { useEffect, useState } from 'react'

import './App.css'
import KpiCard from './components/KpiCard'

interface KPIData {
  totalSales: number;
  totalOrderQuantity:number;
  averageMoneySpent:number
}


function App() {
  const [kpiData, setKpiData] = useState<KPIData | null>(null);

  useEffect(() => {
    const fetchKPIs = async () => {
      try {
        const response = await fetch('http://localhost:5147/kpis');
        const data = await response.json();
        setKpiData(data);
      } catch (error) {
        console.error('Error fetching KPI data:', error);
      }
    };

    fetchKPIs();
  }, []);
  return (
    <div className='min-h-screen'>
      <div className="flex flex-col sm:flex-row justify-between gap-10">
        {kpiData && (
          <>
        <KpiCard
          title="Total Sales"
          value={`$${kpiData.totalSales.toFixed(2)}`}
        />
        
        <KpiCard
          title="Total Order Quantity"
          value={kpiData.totalOrderQuantity.toLocaleString()}
        />
        
        <KpiCard
          title="Average Money Spent"
          value={`$${kpiData.averageMoneySpent.toFixed(2)}`}
        />
          </>
        )}
      </div>
    </div>
  )
}

export default App
