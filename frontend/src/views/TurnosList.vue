<template>
  <div>
    <h2>Turnos</h2>
    <table v-if="turnos.length">
      <thead>
        <tr>
          <th>#</th>
          <th>Paciente</th>
          <th>Médico</th>
          <th>Especialidad</th>
          <th>Fecha y hora</th>
          <th>Estado</th>
          <th>Motivo</th>
          <th>Acciones</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="turno in turnos" :key="turno.id">
          <td>{{ turno.id }}</td>
          <td>{{ turno.paciente?.nombreCompleto }}</td>
          <td>{{ turno.medico?.nombreCompleto }}</td>
          <td>{{ turno.medico?.especialidad }}</td>
          <td>{{ formatFecha(turno.fechaHora) }}</td>
          <td>
            <span :class="['badge', `badge-${turno.estado?.toLowerCase()}`]">{{ turno.estado }}</span>
          </td>
          <td>{{ turno.motivo }}</td>
          <td style="white-space: nowrap">
            <router-link :to="`/turnos/${turno.id}`" class="btn-ver">Ver</router-link>
            <button
              v-if="['Pendiente', 'Confirmado'].includes(turno.estado)"
              class="btn-danger"
              style="margin-left: 6px"
              @click="cancelar(turno.id)"
            >Cancelar</button>
          </td>
        </tr>
      </tbody>
    </table>
    <p v-else>No hay turnos registrados.</p>
  </div>
</template>

<script>
import { turnosApi } from '../services/api'

export default {
  name: 'TurnosList',
  data() {
    return {
      turnos: []
    }
  },
  async mounted() {
    try {
      const res = await turnosApi.getAll()
      this.turnos = res.data
    } catch {
      alert('Error al procesar la solicitud')
    }
  },
  methods: {
    formatFecha(fecha) {
      return new Date(fecha).toLocaleString('es-AR')
    },
    async cancelar(id) {
      try {
        await turnosApi.cancelar(id)
        const res = await turnosApi.getAll()
        this.turnos = res.data
      } catch (e) {
        alert(e.response?.data?.mensaje || 'Error al cancelar el turno')
      }
    }
  }
}
</script>

<style scoped>
.badge {
  padding: 3px 10px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 600;
}
.badge-pendiente   { background: #fff3cd; color: #856404; }
.badge-confirmado  { background: #d4edda; color: #155724; }
.badge-cancelado   { background: #f8d7da; color: #721c24; }
.badge-atendido    { background: #d1ecf1; color: #0c5460; }
.badge-noshow      { background: #e2e3e5; color: #383d41; }

.btn-ver {
  display: inline-block;
  padding: 6px 14px;
  background: #1a73e8;
  color: #fff;
  border-radius: 4px;
  font-size: 13px;
  text-decoration: none;
  font-weight: 500;
}
.btn-ver:hover { background: #1558b0; }
</style>
